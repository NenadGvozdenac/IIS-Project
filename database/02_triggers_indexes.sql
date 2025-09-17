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