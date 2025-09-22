CREATE OR REPLACE FUNCTION create_cart_for_customer()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.type = 'customer' THEN
        INSERT INTO cart (created_at, items_number, status, is_current, id_user)
        VALUES (CURRENT_DATE, 0, 'created', TRUE, NEW.id_user);
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_create_cart_for_customer
    AFTER INSERT ON users
    FOR EACH ROW
    EXECUTE FUNCTION create_cart_for_customer();

CREATE OR REPLACE FUNCTION create_new_cart_on_purchase()
RETURNS TRIGGER AS $$
BEGIN
    IF OLD.status != 'bought' AND NEW.status = 'bought' THEN
        UPDATE cart 
        SET is_current = FALSE 
        WHERE id_cart = NEW.id_cart;
        
        INSERT INTO cart (created_at, items_number, status, is_current, id_user)
        VALUES (CURRENT_DATE, 0, 'created', TRUE, NEW.id_user);
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_create_new_cart_on_purchase
    AFTER UPDATE ON cart
    FOR EACH ROW
    EXECUTE FUNCTION create_new_cart_on_purchase();

CREATE OR REPLACE FUNCTION update_cart_items_count()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        UPDATE cart 
        SET items_number = items_number + 1 
        WHERE id_cart = NEW.id_cart;
        
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
        UPDATE cart 
        SET items_number = items_number - 1 
        WHERE id_cart = OLD.id_cart;
        
        RETURN OLD;
    END IF;
    
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_cart_item_insert
    AFTER INSERT ON cart_item
    FOR EACH ROW
    EXECUTE FUNCTION update_cart_items_count();

CREATE TRIGGER trigger_cart_item_delete
    AFTER DELETE ON cart_item
    FOR EACH ROW
    EXECUTE FUNCTION update_cart_items_count();

-- SRDJAN ILIC TRIGGER #4 - Automatsko kreiranje MatchTracking-a kada se kreira Match
CREATE OR REPLACE FUNCTION create_match_tracking_for_match()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO match_tracking (
        start_time,
        end_time,
        tracking_status,
        period_duration,
        current_period,
        period_status,
        period_start_time,
        elapsed_period_time,
        last_pause_start_time,
        total_pause_time_in_period,
        last_update_time,
        our_points,
        opponent_points,
        id_user,
        id_match
    ) VALUES (
        NULL,                                    -- start_time
        NULL,                                    -- end_time (initially set to scheduled time)
        'upcoming',                              -- tracking_status
        600000,                                  -- period_duration (600000 milliseconds = 10 minutes default)
        '1',                                     -- current_period
        'upcoming',                              -- period_status
        NULL,                                    -- period_start_time
        0,                                       -- elapsed_period_time
        NULL,                                    -- last_pause_start_time
        0,                                       -- total_pause_time_in_period
        NULL,                                    -- last_update_time
        0,                                       -- our_points
        0,                                       -- opponent_points
        NULL,                                    -- id_user
        NEW.id_match                             -- id_match
    );
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_create_match_tracking_for_match
    AFTER INSERT ON match
    FOR EACH ROW
    EXECUTE FUNCTION create_match_tracking_for_match();

-- KRAJ TRIGGERA #4

CREATE OR REPLACE FUNCTION set_cart_item_valid_from()
RETURNS TRIGGER AS $$
DECLARE
    item_record RECORD;
    latest_match_date DATE;
BEGIN
    IF OLD.status != 'bought' AND NEW.status = 'bought' THEN
        FOR item_record IN 
            SELECT ci.id_cart, ci.id_purchase_offer, po.type, po.id_seat
            FROM cart_item ci
            JOIN purchase_offer po ON ci.id_purchase_offer = po.id_purchase_offer
            WHERE ci.id_cart = NEW.id_cart
        LOOP
            IF item_record.type = 'individual ticket' THEN
                UPDATE cart_item 
                SET valid_from = NULL 
                WHERE id_cart = item_record.id_cart 
                AND id_purchase_offer = item_record.id_purchase_offer;
            
            ELSIF item_record.type = 'season ticket' THEN
                SELECT MAX(m.scheduled_at::DATE)
                INTO latest_match_date
                FROM match m
                JOIN individual_ticket it ON it.id_match = m.id_match
                JOIN purchase_offer po_individual ON po_individual.id_purchase_offer = it.id_purchase_offer
                WHERE po_individual.id_seat = item_record.id_seat
                AND po_individual.type = 'individual ticket'
                AND po_individual.status = 'bought'
                AND m.scheduled_at > CURRENT_TIMESTAMP;
                
                IF latest_match_date IS NOT NULL THEN
                    UPDATE cart_item 
                    SET valid_from = latest_match_date + INTERVAL '1 day'
                    WHERE id_cart = item_record.id_cart 
                    AND id_purchase_offer = item_record.id_purchase_offer;
                ELSE
                    UPDATE cart_item 
                    SET valid_from = CURRENT_DATE
                    WHERE id_cart = item_record.id_cart 
                    AND id_purchase_offer = item_record.id_purchase_offer;
                END IF;
                
            END IF;
        END LOOP;
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_set_cart_item_valid_from
    AFTER UPDATE ON cart
    FOR EACH ROW
    EXECUTE FUNCTION set_cart_item_valid_from();

--------------------------------------------------------------------------
-- NENAD GVOZDENAC - PLSQL FUNKCIJA ZA DINAMIČKO IZRAČUNAVANJE CENE KARATA

-- Implementira formulu: P_zona(t) = [P_min + (P_max - P_min) * ((e^(α*O_zona/K_zona + β*O_stad/C_stad) - 1) / (e^(α + β) - 1))] * w_vreme
-- gde su: α = F * k, β = (1 - F) * k
-- w_vreme = 1 + γ * (1 - (D_utakmice - D_trenutno) / T)

CREATE OR REPLACE FUNCTION calculate_ticket_price(
    p_match_id INTEGER,
    p_zone_id INTEGER
) RETURNS NUMERIC AS $$
DECLARE
    -- Parametri iz tabele ticket_price_parameter
    v_price_factor NUMERIC;
    v_time_factor NUMERIC;
    v_min_price NUMERIC;
    v_max_price NUMERIC;
    
    -- Podaci o zoni i stadionu
    v_zone_occupied INTEGER := 0;
    v_zone_capacity INTEGER;
    v_stadium_occupied INTEGER := 0;
    v_stadium_capacity INTEGER;
    
    -- Datum utakmice
    v_match_date DATE;
    v_current_date DATE := CURRENT_DATE;
    v_days_difference INTEGER;
    
    -- Faktori za formulu
    v_f NUMERIC; -- Ponder između zone i stadiona
    v_k NUMERIC; -- Globalni parametar strmine
    v_alpha NUMERIC;
    v_beta NUMERIC;
    v_gamma NUMERIC; -- Vremenski faktor multiplikator
    v_t NUMERIC; -- Maksimalni broj dana za vremenski faktor
    
    -- Rezultati izračuna
    v_zone_ratio NUMERIC;
    v_stadium_ratio NUMERIC;
    v_exponential_factor NUMERIC;
    v_base_price NUMERIC;
    v_time_weight NUMERIC;
    v_final_price NUMERIC;
    
BEGIN
    -- Parametri cena iz tabele
    SELECT 
        tpp.price_factor,
        tpp.time_factor,
        tpp.minimum_seat_price,
        tpp.maximum_seat_price
    INTO 
        v_price_factor,
        v_time_factor,
        v_min_price,
        v_max_price
    FROM ticket_price_parameter tpp
    WHERE tpp.id_match = p_match_id 
      AND tpp.id_zone = p_zone_id
    LIMIT 1;
    
    -- Ako nema parametara, vrati osnovnu cenu
    IF v_min_price IS NULL THEN
        RETURN 1000; -- Osnovna cena od 1000 dinara
    END IF;
    
    -- Kapaciteti zone
    SELECT z.maximum_capacity
    INTO v_zone_capacity
    FROM zone z
    WHERE z.id_zone = p_zone_id;
    
    -- Izračunavanje zauzetih mesta u zoni
    SELECT COUNT(*)
    INTO v_zone_occupied
    FROM cart_item ci
    JOIN purchase_offer po ON ci.id_purchase_offer = po.id_purchase_offer
    JOIN seat s ON po.id_seat = s.id_seat
    JOIN individual_ticket it ON po.id_purchase_offer = it.id_purchase_offer
    WHERE it.id_match = p_match_id 
      AND s.id_zone = p_zone_id
      AND po.status = 'bought';
    
    -- Izračunavanje ukupnog kapaciteta stadiona
    SELECT SUM(z.maximum_capacity)
    INTO v_stadium_capacity
    FROM zone z
    WHERE z.status = 'enabled';
    
    -- Izračunavanje ukupno zauzetih mesta u stadionu za taj meč
    SELECT COUNT(*)
    INTO v_stadium_occupied
    FROM cart_item ci
    JOIN purchase_offer po ON ci.id_purchase_offer = po.id_purchase_offer
    JOIN individual_ticket it ON po.id_purchase_offer = it.id_purchase_offer
    WHERE it.id_match = p_match_id
      AND po.status = 'bought';
    
    -- Dohvatanje datuma utakmice
    SELECT m.scheduled_at
    INTO v_match_date
    FROM match m
    WHERE m.id_match = p_match_id;
    
    -- Postavljanje faktora
    v_f := 0.6; -- 60% uticaj zone, 40% stadiona
    v_k := COALESCE(v_price_factor::NUMERIC / 100.0, 8.0); -- Iz tabele ili default 8
    v_gamma := COALESCE(v_time_factor::NUMERIC / 100.0, 0.7); -- Iz tabele ili default 0.7
    v_t := 30; -- 30 dana maksimalno za vremenski faktor
    
    -- Izračunavanje α i β
    v_alpha := v_f * v_k;
    v_beta := (1 - v_f) * v_k;
    
    -- Izračunavanje odnosa zauzetosti
    v_zone_ratio := CASE 
        WHEN v_zone_capacity > 0 THEN v_zone_occupied::NUMERIC / v_zone_capacity::NUMERIC
        ELSE 0
    END;
    
    v_stadium_ratio := CASE 
        WHEN v_stadium_capacity > 0 THEN v_stadium_occupied::NUMERIC / v_stadium_capacity::NUMERIC
        ELSE 0
    END;
    
    -- Izračunavanje eksponencijalnog faktora
    -- (e^(α*O_zona/K_zona + β*O_stad/C_stad) - 1) / (e^(α + β) - 1)
    v_exponential_factor := (
        exp(v_alpha * v_zone_ratio + v_beta * v_stadium_ratio) - 1
    ) / (
        exp(v_alpha + v_beta) - 1
    );
    
    -- Izračunavanje osnovne cene zone
    v_base_price := v_min_price + (v_max_price - v_min_price) * v_exponential_factor;
    
    -- Izračunavanje vremenskog faktora
    v_days_difference := v_match_date - v_current_date;
    v_time_weight := 1 + v_gamma * (1 - v_days_difference::NUMERIC / v_t);
    
    -- Osiguravanje da vremenski faktor ne bude manji od 0.5
    v_time_weight := GREATEST(v_time_weight, 0.5);
    
    -- Finalna cena
    v_final_price := v_base_price * v_time_weight;
    
    -- Osiguravanje da cena ne prelazi maksimum i nije manja od minimuma
    v_final_price := GREATEST(LEAST(v_final_price, v_max_price * 2), v_min_price * 0.5);
    
    RETURN ROUND(v_final_price, 2);
    
EXCEPTION
    WHEN OTHERS THEN
        -- U slučaju greške, vrati osnovnu cenu
        RETURN COALESCE(v_min_price, 1000);
END;
$$ LANGUAGE plpgsql;

--- KRAJ FUNKCIJE NENAD GVOZDENAC

--------------------------------------------------------------------------
-- NENAD GVOZDENAC - PLSQL TRIGGER FUNKCIJA ZA AGREGACIJU PRODAJE KARATA PO ZONAMA
CREATE OR REPLACE FUNCTION aggregate_match_zone_sales()
RETURNS TRIGGER AS $$
BEGIN
    -- Proverava da li je tickets_for_sale promenjeno sa TRUE na FALSE
    -- što označava da je prodaja karata završena za meč
    IF OLD.tickets_for_sale = TRUE AND NEW.tickets_for_sale = FALSE THEN

        -- Briše postojeće podatke za ovaj meč ako postoje
        DELETE FROM match_zone_sales_summary WHERE id_match = NEW.id_match;

        -- Kreira unose za sve zone sa parametrima cena
        INSERT INTO match_zone_sales_summary (id_match, id_zone, id_ticket_price_parameter, total_tickets_sold, total_revenue)
        SELECT 
            NEW.id_match,
            z.id_zone,
            tpp.id_ticket_price_parameter,
            0 as total_tickets_sold,
            0.00 as total_revenue
        FROM zone z
        LEFT JOIN ticket_price_parameter tpp ON tpp.id_zone = z.id_zone AND tpp.id_match = NEW.id_match
        WHERE z.status = 'enabled'
        ORDER BY z.id_zone;

        -- Priprema podatke o prodaji po zonama
        WITH zone_sales AS (
            SELECT 
                s.id_zone,
                COUNT(*) as ticket_count,
                SUM(ci.price) as revenue
            FROM cart_item ci
            JOIN purchase_offer po ON ci.id_purchase_offer = po.id_purchase_offer
            JOIN individual_ticket it ON po.id_purchase_offer = it.id_purchase_offer
            JOIN seat s ON po.id_seat = s.id_seat
            JOIN cart c ON ci.id_cart = c.id_cart
            WHERE it.id_match = NEW.id_match 
              AND c.status = 'bought'
              AND po.type = 'individual ticket'
            GROUP BY s.id_zone
        )

        -- Ažurira podatke o prodaji za zone koje imaju prodane karte
        UPDATE match_zone_sales_summary 
        SET 
            total_tickets_sold = zone_sales.ticket_count,
            total_revenue = zone_sales.revenue
        FROM zone_sales
        WHERE match_zone_sales_summary.id_match = NEW.id_match 
          AND match_zone_sales_summary.id_zone = zone_sales.id_zone;
        
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER match_finished_aggregation_trigger
    AFTER UPDATE ON match
    FOR EACH ROW
    EXECUTE FUNCTION aggregate_match_zone_sales();

--- KRAJ TRIGGERA NENAD GVOZDENAC

-- Indexes for event tables (ensure present): speed up queries by match id, team, and player
-- This block is idempotent: it checks for existing index names before creating
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_class c WHERE c.relkind = 'i' AND c.relname = 'idx_personal_event_id_match'
    ) THEN
        EXECUTE 'CREATE INDEX idx_personal_event_id_match ON personal_event (id_match)';
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_class c WHERE c.relkind = 'i' AND c.relname = 'idx_team_event_id_match'
    ) THEN
        EXECUTE 'CREATE INDEX idx_team_event_id_match ON team_event (id_match)';
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_class c WHERE c.relkind = 'i' AND c.relname = 'idx_general_event_id_match'
    ) THEN
        EXECUTE 'CREATE INDEX idx_general_event_id_match ON general_event (id_match)';
    END IF;

    -- Player and team statistics indexes for personal_event
    IF NOT EXISTS (
        SELECT 1 FROM pg_class c WHERE c.relkind = 'i' AND c.relname = 'idx_personal_event_id_player'
    ) THEN
        EXECUTE 'CREATE INDEX idx_personal_event_id_player ON personal_event (id_player)';
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_class c WHERE c.relkind = 'i' AND c.relname = 'idx_personal_event_id_team'
    ) THEN
        EXECUTE 'CREATE INDEX idx_personal_event_id_team ON personal_event (id_team)';
    END IF;

    -- Optional: player statistics across all matches for a team
    IF NOT EXISTS (
        SELECT 1 FROM pg_class c WHERE c.relkind = 'i' AND c.relname = 'idx_personal_event_team_player'
    ) THEN
        EXECUTE 'CREATE INDEX idx_personal_event_team_player ON personal_event (id_team, id_player)';
    END IF;

    -- Performance optimization: index on event type for quick filtering by action type
    IF NOT EXISTS (
        SELECT 1 FROM pg_class c WHERE c.relkind = 'i' AND c.relname = 'idx_personal_event_type'
    ) THEN
        EXECUTE 'CREATE INDEX idx_personal_event_type ON personal_event (type)';
    END IF;
END
$$;

------------------------------------------------------------------
-- NENAD GVOZDENAC - INDEXI ZA PERFORMANSE
CREATE INDEX IF NOT EXISTS idx_seat_zone_direction ON seat (id_zone, direction);
CREATE INDEX IF NOT EXISTS idx_seat_zone_direction_row_number ON seat (id_zone, direction, "row", "number");
CREATE INDEX IF NOT EXISTS idx_purchase_offer_seat_type ON purchase_offer (id_seat, type);
CREATE INDEX IF NOT EXISTS idx_purchase_offer_seat_type_status ON purchase_offer (id_seat, type, status);
CREATE INDEX IF NOT EXISTS idx_individual_ticket_purchase_offer ON individual_ticket (id_purchase_offer);
CREATE INDEX IF NOT EXISTS idx_individual_ticket_match ON individual_ticket (id_match);
CREATE INDEX IF NOT EXISTS idx_cart_status ON cart (status);
CREATE INDEX IF NOT EXISTS idx_cart_item_purchase_offer ON cart_item (id_purchase_offer);
CREATE INDEX IF NOT EXISTS idx_cart_user_status ON cart (id_user, status);
CREATE INDEX IF NOT EXISTS idx_match_scheduled_at ON match (scheduled_at);
CREATE INDEX IF NOT EXISTS idx_ticket_price_parameter_match_zone ON ticket_price_parameter (id_match, id_zone);
CREATE INDEX IF NOT EXISTS idx_zone_status ON zone (status);
CREATE INDEX IF NOT EXISTS idx_cart_item_cart_offer ON cart_item (id_cart, id_purchase_offer);

-- KRAJ INDEXA NENAD GVOZDENAC

-- SANJA RADIC - INDEXI 
-- Dobavljanje podataka o zahtevima i ponudama stalno iziskuju pretragu po type i id_match
CREATE INDEX IF NOT EXISTS idx_offer_type_match ON offer (type, id_match);

-- Na 2 stranice za prikaz detalja meca moram ukupno 4 puta da pristupam 
-- bazi za pretragu bas te izabrane ponude za taj mec i odvojeno za prevoz i za smestaj
-- Uvek su samo 2 polja za chosen TRUE, a ostale FALSE
CREATE INDEX IF NOT EXISTS idx_offer_type_match_chosen ON offer (type, id_match, chosen);

-- Ovo je samo rezervni indeks jer se cesto pretrazuje ponuda po tipu
CREATE INDEX IF NOT EXISTS idx_offer_type ON offer (type);

-- Svaki put kad se kreira nova ponuda, mora da se prodje kroz sve agencije
-- i da se uzmu samo one koje su bile cekirane u tom request-u
CREATE INDEX IF NOT EXISTS idx_request_match_type ON request (id_match, type);


-- KRAJ SEKCIJE SA INDEKSIMA SANJA RADIC

-- SANJA RADIC - TRIGGER ZA AUTOMATSKO BIRANJE NAJBOLJE PONUDE
CREATE OR REPLACE FUNCTION auto_select_best_offer()
RETURNS TRIGGER AS $$
DECLARE
    p_match_id INTEGER;
    p_offer_type VARCHAR(20);
    p_weight_price NUMERIC := 0.4;
    p_weight_capacity NUMERIC := 0.3;
    p_weight_benefits NUMERIC := 0.2;
    p_weight_agency NUMERIC := 0.1;

    v_offer_record RECORD;
    v_best_offer_id INTEGER;
    v_best_agency_id INTEGER;
    v_best_request_id INTEGER;
    v_best_score NUMERIC := -1;
    v_best_agency_name VARCHAR(255);
    v_best_reason TEXT;
    v_min_price NUMERIC;
    v_max_price NUMERIC;
    v_min_capacity INTEGER;
    v_max_capacity INTEGER;
    v_current_score NUMERIC;
    v_price_score NUMERIC;
    v_capacity_score NUMERIC;
    v_benefits_score NUMERIC;
    v_agency_score NUMERIC;
    v_benefits_count INTEGER;
    v_agency_success_rate NUMERIC;
    v_total_offers INTEGER;
    v_player_count INTEGER;
    v_capacity_adequacy_score NUMERIC;
BEGIN
    IF current_setting('auto_select.in_progress', true) = '1' THEN
        RETURN NEW;
    END IF;
    PERFORM set_config('auto_select.in_progress', '1', true);

    IF (TG_TABLE_NAME = 'offer') THEN
        p_match_id := NEW.id_match;
        p_offer_type := NEW.type;
    ELSE
        SELECT id_match, type INTO p_match_id, p_offer_type
        FROM offer 
        WHERE id_offer = NEW.id_offer AND id_agency = NEW.id_agency AND id_request = NEW.id_request;
    END IF;

    SELECT COUNT(*) INTO v_total_offers
    FROM offer 
    WHERE id_match = p_match_id AND type = p_offer_type;
    
    IF v_total_offers = 0 THEN
        PERFORM set_config('auto_select.in_progress', '0', true);
        RETURN NEW;
    END IF;

    IF v_total_offers = 1 THEN
        SELECT o.id_offer, o.id_agency, o.id_request, a.name
        INTO v_best_offer_id, v_best_agency_id, v_best_request_id, v_best_agency_name
        FROM offer o
        JOIN sent_request sr ON o.id_request = sr.id_request AND o.id_agency = sr.id_agency
        JOIN agency a ON sr.id_agency = a.id_agency
        WHERE o.id_match = p_match_id AND o.type = p_offer_type
        LIMIT 1;
        
        IF v_best_offer_id IS NOT NULL THEN
            UPDATE offer 
            SET chosen = true 
            WHERE id_offer = v_best_offer_id 
            AND id_agency = v_best_agency_id 
            AND id_request = v_best_request_id;
            
            INSERT INTO offer_selection_log (id_match, offer_type, selected_offer_id, selection_score, selection_reason, total_offers_analyzed, selected_by)
            VALUES (p_match_id, p_offer_type, v_best_offer_id, 100.0, 'Only one offer available - automatically selected', v_total_offers, 'AUTO');
        END IF;

        PERFORM set_config('auto_select.in_progress', '0', true);
        RETURN NEW;
    END IF;

    v_player_count := 25;

    SELECT MIN(price), MAX(price) INTO v_min_price, v_max_price
    FROM offer 
    WHERE id_match = p_match_id AND type = p_offer_type;

    IF p_offer_type = 'transportation' THEN
        SELECT MIN(capacity), MAX(capacity) INTO v_min_capacity, v_max_capacity
        FROM offer o
        JOIN transportation_offer to_obj ON o.id_offer = to_obj.id_offer 
          AND o.id_agency = to_obj.id_agency AND o.id_request = to_obj.id_request
        WHERE o.id_match = p_match_id AND o.type = p_offer_type;
    ELSE
        SELECT MIN(capacity), MAX(capacity) INTO v_min_capacity, v_max_capacity
        FROM offer o
        JOIN accommodation_offer ao ON o.id_offer = ao.id_offer 
          AND o.id_agency = ao.id_agency AND o.id_request = ao.id_request
        WHERE o.id_match = p_match_id AND o.type = p_offer_type;
    END IF;

    RAISE NOTICE 'DEBUG auto_select: Starting main loop with % total offers for match=% type=%', v_total_offers, p_match_id, p_offer_type;
    FOR v_offer_record IN 
        SELECT 
            o.id_offer,
            o.price,
            o.id_agency,
            o.id_request,
            CASE 
                WHEN p_offer_type = 'transportation' THEN to_obj.capacity
                ELSE ao.capacity
            END as capacity,
            a.name as agency_name
        FROM offer o
        LEFT JOIN transportation_offer to_obj ON o.id_offer = to_obj.id_offer 
          AND o.id_agency = to_obj.id_agency AND o.id_request = to_obj.id_request
        LEFT JOIN accommodation_offer ao ON o.id_offer = ao.id_offer 
          AND o.id_agency = ao.id_agency AND o.id_request = ao.id_request
        JOIN sent_request sr ON o.id_request = sr.id_request AND o.id_agency = sr.id_agency
        JOIN agency a ON sr.id_agency = a.id_agency
        WHERE o.id_match = p_match_id AND o.type = p_offer_type
    LOOP
        SELECT calculate_advanced_offer_score(v_offer_record.id_offer, v_offer_record.id_agency, v_offer_record.id_request)
        INTO v_current_score;
        RAISE NOTICE 'DEBUG auto_select: Offer % (agency %) - price=%, capacity=%, score=%', 
            v_offer_record.id_offer, v_offer_record.id_agency, v_offer_record.price, v_offer_record.capacity, 
            v_current_score;

        IF v_current_score > v_best_score THEN
            v_best_score := v_current_score;
            v_best_offer_id := v_offer_record.id_offer;
            v_best_agency_id := v_offer_record.id_agency;
            v_best_request_id := v_offer_record.id_request;
            v_best_agency_name := v_offer_record.agency_name;
            v_best_reason := 'AUTO selection - calculated by calculate_advanced_offer_score';
            
            RAISE NOTICE 'DEBUG auto_select: NEW BEST OFFER found: % with score %', v_best_offer_id, v_best_score;
        END IF;
    END LOOP;

    IF v_best_offer_id IS NOT NULL THEN
        UPDATE offer 
        SET chosen = false 
        WHERE id_match = p_match_id AND type = p_offer_type;
        
        UPDATE offer 
        SET chosen = true 
        WHERE id_offer = v_best_offer_id 
        AND id_agency = v_best_agency_id 
        AND id_request = v_best_request_id;
        
        INSERT INTO offer_selection_log (id_match, offer_type, selected_offer_id, selection_score, selection_reason, total_offers_analyzed, selected_by)
        VALUES (p_match_id, p_offer_type, v_best_offer_id, v_best_score, v_best_reason, v_total_offers, 'AUTO');
    END IF;

    PERFORM set_config('auto_select.in_progress', '0', true);
    RETURN NEW;

EXCEPTION
    WHEN OTHERS THEN
        PERFORM set_config('auto_select.in_progress', '0', true);
        RAISE;
END;
$$ LANGUAGE plpgsql;


CREATE TRIGGER trigger_auto_select_after_offer_insert
    AFTER INSERT ON offer
    FOR EACH ROW
    EXECUTE FUNCTION auto_select_best_offer();
CREATE TRIGGER trigger_auto_select_after_transportation_child
    AFTER INSERT OR UPDATE ON transportation_offer
    FOR EACH ROW
    EXECUTE FUNCTION auto_select_best_offer();

CREATE TRIGGER trigger_auto_select_after_accommodation_child
    AFTER INSERT OR UPDATE ON accommodation_offer
    FOR EACH ROW
    EXECUTE FUNCTION auto_select_best_offer();
-- KRAJ TRIGGERA SANJA RADIC

-- SANJA RADIC - NAPREDNA FUNKCIJA ZA SCORING PONUDA
-- Ova funkcija je potrebna samo zbog racunanja score-a
CREATE OR REPLACE FUNCTION calculate_total_travelers(p_request_id INTEGER)
RETURNS INTEGER AS $$
DECLARE
    v_team_members INTEGER := 0;
    v_management_members INTEGER := 0;
BEGIN
    BEGIN
        SELECT COUNT(*) INTO v_team_members FROM team_member_request WHERE id_request = p_request_id;
    EXCEPTION WHEN OTHERS THEN
        v_team_members := 0;
    END;

    BEGIN
        SELECT COUNT(*) INTO v_management_members FROM management_member_request WHERE id_request = p_request_id;
    EXCEPTION WHEN OTHERS THEN
        v_management_members := 0;
    END;

    RETURN COALESCE(v_team_members, 0) + COALESCE(v_management_members, 0);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION calculate_advanced_offer_score(
    p_offer_id INTEGER,
    p_agency_id INTEGER, 
    p_request_id INTEGER,
    p_weight_price NUMERIC DEFAULT 0.3,
    p_weight_capacity NUMERIC DEFAULT 0.2,
    p_weight_amenities NUMERIC DEFAULT 0.25,
    p_weight_agency_rating NUMERIC DEFAULT 0.25
) RETURNS NUMERIC AS $$
DECLARE
    v_offer_type VARCHAR(20);
    v_offer_price NUMERIC;
    v_min_price NUMERIC;
    v_max_price NUMERIC;
    v_price_score NUMERIC := 0;
    v_capacity_score NUMERIC := 0;
    v_amenities_score NUMERIC := 0;
    v_agency_rating_score NUMERIC := 0;
    v_final_score NUMERIC := 0;
    v_offer_capacity INTEGER;
    v_required_capacity INTEGER;
    v_amenities_count INTEGER := 0;
    v_max_amenities INTEGER;
    v_match_id INTEGER;
BEGIN
    SELECT o.type, o.price, o.id_match 
    INTO v_offer_type, v_offer_price, v_match_id
    FROM offer o 
    WHERE o.id_offer = p_offer_id 
    AND o.id_agency = p_agency_id 
    AND o.id_request = p_request_id;
    
    IF v_offer_type IS NULL THEN
        RETURN 0;
    END IF;
    
    SELECT MIN(price), MAX(price) 
    INTO v_min_price, v_max_price
    FROM offer 
    WHERE type = v_offer_type 
    AND id_match = v_match_id 
    AND price IS NOT NULL;
    
    IF v_offer_price IS NOT NULL AND v_max_price > v_min_price THEN
        v_price_score := 100 * (1 - ((v_offer_price - v_min_price) / (v_max_price - v_min_price)));
    ELSE
        v_price_score := 50;
    END IF;
    
    SELECT calculate_total_travelers(p_request_id) INTO v_required_capacity;
    
    IF v_offer_type = 'accommodation' THEN
        SELECT capacity INTO v_offer_capacity
        FROM accommodation_offer 
        WHERE id_offer = p_offer_id AND id_agency = p_agency_id AND id_request = p_request_id;
    ELSIF v_offer_type = 'transportation' THEN
        SELECT capacity INTO v_offer_capacity
        FROM transportation_offer 
        WHERE id_offer = p_offer_id AND id_agency = p_agency_id AND id_request = p_request_id;
    END IF;
    
    IF v_offer_capacity IS NOT NULL AND v_required_capacity > 0 THEN
        IF v_offer_capacity >= v_required_capacity THEN
            v_capacity_score := 100 * (1 - EXP(-2.0 * v_offer_capacity::NUMERIC / v_required_capacity::NUMERIC));
        ELSE
            v_capacity_score := 0;
        END IF;
    ELSE
        v_capacity_score := 50;
    END IF;
    
    IF v_offer_type = 'accommodation' THEN
        SELECT 
            (CASE WHEN breakfast THEN 1 ELSE 0 END) +
            (CASE WHEN fitness_center THEN 1 ELSE 0 END) +
            (CASE WHEN pool THEN 1 ELSE 0 END) +
            (CASE WHEN wifi THEN 1 ELSE 0 END) +
            (CASE WHEN spa THEN 1 ELSE 0 END) +
            (CASE WHEN double_room THEN 1 ELSE 0 END) +
            (CASE WHEN triple_room THEN 1 ELSE 0 END) +
            (CASE WHEN quadruple_room THEN 1 ELSE 0 END)
        INTO v_amenities_count
        FROM accommodation_offer 
        WHERE id_offer = p_offer_id AND id_agency = p_agency_id AND id_request = p_request_id;
        
        v_max_amenities := 8;
        
    ELSIF v_offer_type = 'transportation' THEN
        SELECT 
            (CASE WHEN equipment_space THEN 1 ELSE 0 END) +
            (CASE WHEN air_conditioning THEN 1 ELSE 0 END) +
            (CASE WHEN tv THEN 1 ELSE 0 END) +
            (CASE WHEN wifi THEN 1 ELSE 0 END) +
            (CASE WHEN restroom THEN 1 ELSE 0 END)
        INTO v_amenities_count
        FROM transportation_offer 
        WHERE id_offer = p_offer_id AND id_agency = p_agency_id AND id_request = p_request_id;
        
        v_max_amenities := 5;
    END IF;
    
    IF v_max_amenities > 0 THEN
        v_amenities_score := 100 * (v_amenities_count::NUMERIC / v_max_amenities::NUMERIC);
    ELSE
        v_amenities_score := 0;
    END IF;
    
    v_agency_rating_score := 50; 
    
    v_final_score := 
        (v_price_score * p_weight_price) +
        (v_capacity_score * p_weight_capacity) +
        (v_amenities_score * p_weight_amenities) +
        (v_agency_rating_score * p_weight_agency_rating);
    
    RETURN ROUND(v_final_score, 3);
    
EXCEPTION
    WHEN OTHERS THEN
        RETURN 0;
END;
$$ LANGUAGE plpgsql;

-- KRAJ NAPREDNE FUNKCIJE ZA SCORING PONUDA SANJA RADIC