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

------------------------------------------------------------------
-- NENAD GVOZDENAC - INDEXI ZA PERFORMANSE

-- Kompozicioni indeks za pretragu sedišta po zoni i smeru sa sortiranjem po redu i broju
-- Optimizuje glavnu pretragu u GetSeatsWithOffersForZoneAndDirection koja filtrira sedišta po zoni i smeru, 
-- a zatim ih sortira po redu i broju sedišta
CREATE INDEX IF NOT EXISTS idx_seat_zone_direction_row_number ON seat (id_zone, direction, "row", "number");
-- EXPLAIN ANALYZE SELECT * FROM seat WHERE id_zone = 1 AND direction = 'north' ORDER BY "row", "number";

-- Kompozicioni indeks za filtriranje ponuda po sedištu, tipu i statusu
-- Bitan za pretragu individualnih ponuda i sezonskih karata u GetSeatsWithOffersForZoneAndDirection
-- koji filtrira ponude po sedištu, zatim po tipu ('individual ticket', 'season ticket') i statusu ('enabled', 'bought')
CREATE INDEX IF NOT EXISTS idx_purchase_offer_seat_type_status ON purchase_offer (id_seat, type, status);
-- EXPLAIN ANALYZE SELECT * FROM purchase_offer WHERE id_seat > 1 and id_seat < 100 AND type = 'individual ticket' AND status = 'enabled';

-- Indeks za pretragu individualnih karata po meču
-- Omogućava dobavljanje svih individualnih karata za određeni meč u GetSeatsWithOffersForZoneAndDirection
CREATE INDEX IF NOT EXISTS idx_individual_ticket_match ON individual_ticket (id_match);
-- EXPLAIN ANALYZE SELECT * FROM individual_ticket WHERE id_match = 1;

-- Indeks za filtriranje Korpa po statusu (potreban za sezonske karte)
-- Optimizuje pretragu Korpa sa statusom 'bought' ili 'active' kada se proveravaju konflikti sezonskih karata
CREATE INDEX IF NOT EXISTS idx_cart_status ON cart (status);
-- EXPLAIN ANALYZE SELECT * FROM cart WHERE status IN ('bought', 'active');

-- Indeks za povezivanje stavki Korpe sa ponudama
-- Omogućava brzu pretragu stavki Korpe po ID ponude za proveru sezonskih karata
CREATE INDEX IF NOT EXISTS idx_cart_item_purchase_offer ON cart_item (id_purchase_offer);
-- EXPLAIN ANALYZE SELECT * FROM cart_item WHERE id_purchase_offer IN (1, 2, 3);

-- Indeks za kalkulaciju cene karte po meču i zoni
-- Koristi se za dinamičko izračunavanje cene karte u TicketPriceCalculationService
CREATE INDEX IF NOT EXISTS idx_ticket_price_parameter_match_zone ON ticket_price_parameter (id_match, id_zone);
-- EXPLAIN ANALYZE SELECT * FROM ticket_price_parameter WHERE id_match = 1 AND id_zone = 2;

-- KRAJ INDEXA NENAD GVOZDENAC

------------------------------------------------------------------
--- NENAD GVOZDENAC - SLOZENI TIPOVI, KURSORI I IZVESTAJ
-- Složeni tip koji predstavlja red u izvještaju
CREATE TYPE match_summary_row AS (
    match_id INTEGER,
    match_name VARCHAR(255),
    match_date TIMESTAMP WITH TIME ZONE,
    match_type VARCHAR(20),
    city VARCHAR(255),
    hall VARCHAR(255),
    season_name VARCHAR(255),
    competition_name VARCHAR(255),
    team_name VARCHAR(255),
    total_tickets_sold INTEGER,
    total_revenue DECIMAL(12,2),
    vip_zone_tickets INTEGER,
    vip_zone_revenue DECIMAL(12,2),
    regular_zone_tickets INTEGER,
    regular_zone_revenue DECIMAL(12,2),
    average_ticket_price DECIMAL(10,2),
    stadium_fill_percentage DECIMAL(5,2),
    vip_zone_fill_percentage DECIMAL(5,2),
    regular_zone_fill_percentage DECIMAL(5,2),
    highest_selling_zone VARCHAR(255),
    lowest_selling_zone VARCHAR(255),
    tracking_status VARCHAR(20),
    our_points INTEGER,
    opponent_points INTEGER
);

CREATE OR REPLACE FUNCTION generate_match_summary_report()
RETURNS SETOF match_summary_row AS $$
DECLARE
    match_cursor CURSOR FOR
        WITH match_base_data AS (
            SELECT 
                m.id_match,
                m.name as match_name,
                m.scheduled_at,
                m.type as match_type,
                m.city,
                m.hall,
                s.name as season_name,
                c.name as competition_name,
                t.name as team_name,
                mt.tracking_status,
                mt.our_points,
                mt.opponent_points
            FROM match m
            JOIN season s ON m.id_season = s.id_season
            LEFT JOIN competition c ON m.id_competition = c.id_competition
            JOIN team t ON m.id_team = t.id_team
            LEFT JOIN match_tracking mt ON m.id_match = mt.id_match
            WHERE m.tickets_for_sale = FALSE
        ),
        sales_summary AS (
            SELECT 
                mzss.id_match,
                SUM(mzss.total_tickets_sold) as total_tickets_sold,
                SUM(mzss.total_revenue) as total_revenue,
                CASE 
                    WHEN SUM(mzss.total_tickets_sold) > 0 
                    THEN SUM(mzss.total_revenue) / SUM(mzss.total_tickets_sold)
                    ELSE 0 
                END as average_ticket_price
            FROM match_zone_sales_summary mzss
            WHERE mzss.total_tickets_sold > 0
            GROUP BY mzss.id_match
        ),
        zone_details AS (
            SELECT 
                mzss.id_match,
                -- VIP zone podatci (Zone 100 je VIP)
                SUM(CASE WHEN z.rank = 100 THEN mzss.total_tickets_sold ELSE 0 END) as vip_tickets,
                SUM(CASE WHEN z.rank = 100 THEN mzss.total_revenue ELSE 0 END) as vip_revenue,
                -- Regularni zone podatci (rank > 100)
                SUM(CASE WHEN z.rank > 100 THEN mzss.total_tickets_sold ELSE 0 END) as regular_tickets,
                SUM(CASE WHEN z.rank > 100 THEN mzss.total_revenue ELSE 0 END) as regular_revenue
            FROM match_zone_sales_summary mzss
            JOIN zone z ON mzss.id_zone = z.id_zone
            GROUP BY mzss.id_match
        ),
        zone_rankings AS (
            -- Poseban CTE za rangiranje zona po prodaji
            SELECT 
                mzss.id_match,
                -- Zona sa najviše prodanih karata (samo ako ima prodaje)
                (SELECT z2.name 
                 FROM match_zone_sales_summary mzss2 
                 JOIN zone z2 ON mzss2.id_zone = z2.id_zone
                 WHERE mzss2.id_match = mzss.id_match 
                   AND mzss2.total_tickets_sold > 0
                 ORDER BY mzss2.total_tickets_sold DESC 
                 LIMIT 1) as highest_selling_zone,
                -- Zona sa najmanje prodanih karata (samo ako ima više od jedne zone sa prodajom)
                (SELECT z3.name 
                 FROM match_zone_sales_summary mzss3 
                 JOIN zone z3 ON mzss3.id_zone = z3.id_zone
                 WHERE mzss3.id_match = mzss.id_match 
                   AND mzss3.total_tickets_sold > 0
                   AND (SELECT COUNT(*) FROM match_zone_sales_summary mzss4 
                        WHERE mzss4.id_match = mzss.id_match 
                          AND mzss4.total_tickets_sold > 0) > 1
                 ORDER BY mzss3.total_tickets_sold ASC 
                 LIMIT 1) as lowest_selling_zone
            FROM match_zone_sales_summary mzss
            GROUP BY mzss.id_match
        ),
        stadium_capacity AS (
            SELECT 
                SUM(z.maximum_capacity) as total_capacity,
                SUM(CASE WHEN z.rank = 100 THEN z.maximum_capacity ELSE 0 END) as vip_capacity,
                SUM(CASE WHEN z.rank > 100 THEN z.maximum_capacity ELSE 0 END) as regular_capacity
            FROM zone z 
            WHERE z.status = 'enabled'
        )
        SELECT 
            mbd.id_match,
            mbd.match_name,
            mbd.scheduled_at,
            mbd.match_type,
            mbd.city,
            mbd.hall,
            mbd.season_name,
            mbd.competition_name,
            mbd.team_name,
            COALESCE(ss.total_tickets_sold, 0) as total_tickets_sold,
            COALESCE(ss.total_revenue, 0) as total_revenue,
            COALESCE(zd.vip_tickets, 0) as vip_tickets,
            COALESCE(zd.vip_revenue, 0) as vip_revenue,
            COALESCE(zd.regular_tickets, 0) as regular_tickets,
            COALESCE(zd.regular_revenue, 0) as regular_revenue,
            COALESCE(ss.average_ticket_price, 0) as average_ticket_price,
            CASE 
                WHEN sc.total_capacity > 0 
                THEN (COALESCE(ss.total_tickets_sold, 0)::DECIMAL / sc.total_capacity::DECIMAL) * 100
                ELSE 0 
            END as stadium_fill_percentage,
            CASE 
                WHEN sc.vip_capacity > 0 
                THEN (COALESCE(zd.vip_tickets, 0)::DECIMAL / sc.vip_capacity::DECIMAL) * 100
                ELSE 0 
            END as vip_zone_fill_percentage,
            CASE 
                WHEN sc.regular_capacity > 0 
                THEN (COALESCE(zd.regular_tickets, 0)::DECIMAL / sc.regular_capacity::DECIMAL) * 100
                ELSE 0 
            END as regular_zone_fill_percentage,
            COALESCE(zr.highest_selling_zone, 'N/A') as highest_selling_zone,
            COALESCE(zr.lowest_selling_zone, 'N/A') as lowest_selling_zone,
            COALESCE(mbd.tracking_status, 'unknown') as tracking_status,
            COALESCE(mbd.our_points, 0) as our_points,
            COALESCE(mbd.opponent_points, 0) as opponent_points
        FROM match_base_data mbd
        CROSS JOIN stadium_capacity sc
        LEFT JOIN sales_summary ss ON mbd.id_match = ss.id_match
        LEFT JOIN zone_details zd ON mbd.id_match = zd.id_match
        LEFT JOIN zone_rankings zr ON mbd.id_match = zr.id_match
        WHERE COALESCE(ss.total_tickets_sold, 0) >= 0
        ORDER BY mbd.scheduled_at DESC;

    match_record RECORD;
    summary_row match_summary_row;
    
BEGIN
    -- Otvaranje kursora i prolazak kroz sve redove
    FOR match_record IN match_cursor
    LOOP
        -- Kreiranje reda za rezultat
        summary_row.match_id := match_record.id_match;
        summary_row.match_name := match_record.match_name;
        summary_row.match_date := match_record.scheduled_at;
        summary_row.match_type := match_record.match_type;
        summary_row.city := match_record.city;
        summary_row.hall := match_record.hall;
        summary_row.season_name := match_record.season_name;
        summary_row.competition_name := match_record.competition_name;
        summary_row.team_name := match_record.team_name;
        summary_row.total_tickets_sold := match_record.total_tickets_sold;
        summary_row.total_revenue := match_record.total_revenue;
        summary_row.vip_zone_tickets := match_record.vip_tickets;
        summary_row.vip_zone_revenue := match_record.vip_revenue;
        summary_row.regular_zone_tickets := match_record.regular_tickets;
        summary_row.regular_zone_revenue := match_record.regular_revenue;
        summary_row.average_ticket_price := match_record.average_ticket_price;
        summary_row.stadium_fill_percentage := match_record.stadium_fill_percentage;
        summary_row.vip_zone_fill_percentage := match_record.vip_zone_fill_percentage;
        summary_row.regular_zone_fill_percentage := match_record.regular_zone_fill_percentage;
        summary_row.highest_selling_zone := match_record.highest_selling_zone;
        summary_row.lowest_selling_zone := match_record.lowest_selling_zone;
        summary_row.tracking_status := match_record.tracking_status;
        summary_row.our_points := match_record.our_points;
        summary_row.opponent_points := match_record.opponent_points;
        
        RETURN NEXT summary_row;
    END LOOP;
    
    RETURN;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE VIEW match_summary_report_view AS
SELECT * FROM generate_match_summary_report();

-- KRAJ KOMPLEKSNIH TIPOVA, KURSORA I IZVESTAJA NENAD GVOZDENAC

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
    --TESTIRANJE INDEKSA
    --BEZ INDEKSA
        --docker exec -i iis-project-postgres_db-1 psql -U postgres -d sportsdb -c "SET enable_indexscan = off; SET enable_bitmapscan = off; EXPLAIN ANALYZE SELECT * FROM personal_event WHERE id_player = 2; SET enable_indexscan = on; SET enable_bitmapscan = on;"

    --SA INDEKSOM
        --docker exec -i iis-project-postgres_db-1 psql -U postgres -d sportsdb -c "EXPLAIN ANALYZE SELECT * FROM personal_event WHERE id_player = 2;"

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

-- IZVESTAJ O TROSKOVIMA PUTOVANJA SANJA RADIC

-- Slozeni tipovi za izvestaj o troskovima putovanja
CREATE TYPE travel_cost_detail AS (
    match_id INTEGER,
    match_name VARCHAR(255),
    match_date TIMESTAMP WITH TIME ZONE,
    opponent_team VARCHAR(255),
    city VARCHAR(255),
    hall VARCHAR(255),
    transportation_cost INTEGER,
    transportation_agency VARCHAR(255),
    transportation_type VARCHAR(20),
    transportation_company VARCHAR(255),
    accommodation_cost INTEGER,
    accommodation_agency VARCHAR(255),
    accommodation_name VARCHAR(255),
    accommodation_type VARCHAR(20),
    total_match_cost INTEGER,
    team_members_count INTEGER,
    management_members_count INTEGER,
    total_travelers INTEGER
);

CREATE TYPE travel_cost_summary AS (
    total_matches INTEGER,
    total_transportation_cost INTEGER,
    total_accommodation_cost INTEGER,
    total_travel_cost INTEGER,
    average_cost_per_match NUMERIC(10,2),
    matches_with_accommodation INTEGER,
    matches_with_transportation INTEGER
);

CREATE TYPE complete_travel_report AS (
    report_date TIMESTAMP WITH TIME ZONE,
    season_info VARCHAR(255),
    match_details travel_cost_detail[],
    cost_summary travel_cost_summary,
    team_members_by_match TEXT[],
    management_members_by_match TEXT[]
);

-- Glavna funkcija za generiranje izvestaja o troskovima putovanja
CREATE OR REPLACE FUNCTION generate_travel_cost_report(
    p_season_id INTEGER DEFAULT NULL,
    p_start_date DATE DEFAULT NULL,
    p_end_date DATE DEFAULT NULL
) RETURNS complete_travel_report AS $$
DECLARE
    report_result complete_travel_report;
    match_detail travel_cost_detail;
    cost_summary travel_cost_summary;
    match_cursor CURSOR FOR
        WITH away_matches_with_trips AS (
            SELECT DISTINCT
                m.id_match,
                m.name as match_name,
                m.scheduled_at,
                m.city,
                m.hall,
                t.name as opponent_team,
                tr.id_transportation_offer,
                tr.id_transportation_agency,
                tr.id_transportation_request,
                tr.id_accommodation_offer,
                tr.id_accommodation_agency,
                tr.id_accommodation_request
            FROM match m
            INNER JOIN trip tr ON m.id_match = tr.match_id_match
            INNER JOIN team t ON m.id_team = t.id_team
            WHERE m.type = 'away'
            AND (p_season_id IS NULL OR m.id_season = p_season_id)
            AND (p_start_date IS NULL OR DATE(m.scheduled_at) >= p_start_date)
            AND (p_end_date IS NULL OR DATE(m.scheduled_at) <= p_end_date)
            ORDER BY m.scheduled_at
        ),
        match_costs AS (
            SELECT 
                awt.id_match,
                awt.match_name,
                awt.scheduled_at,
                awt.opponent_team,
                awt.city,
                awt.hall,
                COALESCE(to_offer.price, 0) as transport_cost,
                ta.name as transport_agency,
                tro.type as transport_type,
                tro.company_name as transport_company,
                COALESCE(ao_offer.price, 0) as accommodation_cost,
                aa.name as accommodation_agency,
                aco.name as accommodation_name,
                aco.accommodation_type
            FROM away_matches_with_trips awt
            LEFT JOIN offer to_offer ON (
                awt.id_transportation_offer = to_offer.id_offer 
                AND awt.id_transportation_agency = to_offer.id_agency 
                AND awt.id_transportation_request = to_offer.id_request
            )
            LEFT JOIN agency ta ON awt.id_transportation_agency = ta.id_agency
            LEFT JOIN transportation_offer tro ON (
                awt.id_transportation_offer = tro.id_offer 
                AND awt.id_transportation_agency = tro.id_agency 
                AND awt.id_transportation_request = tro.id_request
            )
            LEFT JOIN offer ao_offer ON (
                awt.id_accommodation_offer = ao_offer.id_offer 
                AND awt.id_accommodation_agency = ao_offer.id_agency 
                AND awt.id_accommodation_request = ao_offer.id_request
            )
            LEFT JOIN agency aa ON awt.id_accommodation_agency = aa.id_agency
            LEFT JOIN accommodation_offer aco ON (
                awt.id_accommodation_offer = aco.id_offer 
                AND awt.id_accommodation_agency = aco.id_agency 
                AND awt.id_accommodation_request = aco.id_request
            )
        )
        SELECT 
            mc.id_match,
            mc.match_name,
            mc.scheduled_at,
            mc.opponent_team,
            mc.city,
            mc.hall,
            mc.transport_cost,
            mc.transport_agency,
            mc.transport_type,
            mc.transport_company,
            mc.accommodation_cost,
            mc.accommodation_agency,
            mc.accommodation_name,
            mc.accommodation_type
        FROM match_costs mc;
        
    match_details_array travel_cost_detail[] := '{}';
    team_members_array TEXT[] := '{}';
    management_members_array TEXT[] := '{}';
    
    v_team_members_count INTEGER;
    v_management_members_count INTEGER;
    v_team_members_list TEXT;
    v_management_members_list TEXT;
    
    v_total_matches INTEGER := 0;
    v_total_transport_cost INTEGER := 0;
    v_total_accommodation_cost INTEGER := 0;
    v_matches_with_accommodation INTEGER := 0;
    v_matches_with_transportation INTEGER := 0;
    
BEGIN
    report_result.report_date := NOW();
    
    IF p_season_id IS NOT NULL THEN
        SELECT 'Season: ' || name INTO report_result.season_info 
        FROM season WHERE id_season = p_season_id;
    ELSE
        report_result.season_info := 'All Seasons';
    END IF;
    
    FOR match_rec IN match_cursor LOOP
        SELECT COUNT(DISTINCT tmr.id_player)
        INTO v_team_members_count
        FROM team_member_request tmr
        INNER JOIN request r ON tmr.id_request = r.id_request
        WHERE r.id_match = match_rec.id_match;
        
        SELECT COUNT(DISTINCT mmr.id_management_member)
        INTO v_management_members_count
        FROM management_member_request mmr
        INNER JOIN request r ON mmr.id_request = r.id_request
        WHERE r.id_match = match_rec.id_match;
        
        SELECT STRING_AGG(p.name || ' ' || p.surname, ', ')
        INTO v_team_members_list
        FROM team_member_request tmr
        INNER JOIN request r ON tmr.id_request = r.id_request
        INNER JOIN player p ON tmr.id_player = p.id_player
        WHERE r.id_match = match_rec.id_match;
        
        SELECT STRING_AGG(m.member_name || ' ' || m.member_surname || ' (' || m.member_role || ')', ', ')
        INTO v_management_members_list
        FROM management_member_request mmr
        INNER JOIN request r ON mmr.id_request = r.id_request
        INNER JOIN management m ON mmr.id_management_member = m.member_id
        WHERE r.id_match = match_rec.id_match;
        
        match_detail.match_id := match_rec.id_match;
        match_detail.match_name := match_rec.match_name;
        match_detail.match_date := match_rec.scheduled_at;
        match_detail.opponent_team := match_rec.opponent_team;
        match_detail.city := match_rec.city;
        match_detail.hall := match_rec.hall;
        match_detail.transportation_cost := COALESCE(match_rec.transport_cost, 0);
        match_detail.transportation_agency := match_rec.transport_agency;
        match_detail.transportation_type := match_rec.transport_type;
        match_detail.transportation_company := match_rec.transport_company;
        match_detail.accommodation_cost := COALESCE(match_rec.accommodation_cost, 0);
        match_detail.accommodation_agency := match_rec.accommodation_agency;
        match_detail.accommodation_name := match_rec.accommodation_name;
        match_detail.accommodation_type := match_rec.accommodation_type;
        match_detail.total_match_cost := COALESCE(match_rec.transport_cost, 0) + COALESCE(match_rec.accommodation_cost, 0);
        match_detail.team_members_count := v_team_members_count;
        match_detail.management_members_count := v_management_members_count;
        match_detail.total_travelers := v_team_members_count + v_management_members_count;
        
        match_details_array := array_append(match_details_array, match_detail);
        team_members_array := array_append(team_members_array, 
            'Match ' || match_rec.id_match || ' (' || match_rec.match_name || '): ' || COALESCE(v_team_members_list, 'No team members'));
        management_members_array := array_append(management_members_array, 
            'Match ' || match_rec.id_match || ' (' || match_rec.match_name || '): ' || COALESCE(v_management_members_list, 'No management members'));
        
        v_total_matches := v_total_matches + 1;
        v_total_transport_cost := v_total_transport_cost + COALESCE(match_rec.transport_cost, 0);
        v_total_accommodation_cost := v_total_accommodation_cost + COALESCE(match_rec.accommodation_cost, 0);
        
        IF match_rec.accommodation_cost IS NOT NULL AND match_rec.accommodation_cost > 0 THEN
            v_matches_with_accommodation := v_matches_with_accommodation + 1;
        END IF;
        
        IF match_rec.transport_cost IS NOT NULL AND match_rec.transport_cost > 0 THEN
            v_matches_with_transportation := v_matches_with_transportation + 1;
        END IF;
    END LOOP;
    
    WITH cost_aggregation AS (
        SELECT 
            COUNT(*) as total_count,
            SUM(CASE WHEN transportation_cost > 0 THEN transportation_cost ELSE 0 END) as total_transport,
            SUM(CASE WHEN accommodation_cost > 0 THEN accommodation_cost ELSE 0 END) as total_accommodation,
            COUNT(CASE WHEN transportation_cost > 0 THEN 1 END) as transport_matches,
            COUNT(CASE WHEN accommodation_cost > 0 THEN 1 END) as accommodation_matches
        FROM unnest(match_details_array) as match_data
        GROUP BY ()
        HAVING COUNT(*) > 0
    )
    SELECT 
        total_count,
        total_transport,
        total_accommodation,
        total_transport + total_accommodation,
        CASE WHEN total_count > 0 THEN (total_transport + total_accommodation)::NUMERIC / total_count ELSE 0 END,
        accommodation_matches,
        transport_matches
    INTO 
        cost_summary.total_matches,
        cost_summary.total_transportation_cost,
        cost_summary.total_accommodation_cost,
        cost_summary.total_travel_cost,
        cost_summary.average_cost_per_match,
        cost_summary.matches_with_accommodation,
        cost_summary.matches_with_transportation
    FROM cost_aggregation;
    
    IF cost_summary.total_matches IS NULL THEN
        cost_summary.total_matches := 0;
        cost_summary.total_transportation_cost := 0;
        cost_summary.total_accommodation_cost := 0;
        cost_summary.total_travel_cost := 0;
        cost_summary.average_cost_per_match := 0;
        cost_summary.matches_with_accommodation := 0;
        cost_summary.matches_with_transportation := 0;
    END IF;
    
    report_result.match_details := match_details_array;
    report_result.cost_summary := cost_summary;
    report_result.team_members_by_match := team_members_array;
    report_result.management_members_by_match := management_members_array;
    
    RETURN report_result;
    
EXCEPTION
    WHEN OTHERS THEN
        report_result.report_date := NOW();
        report_result.season_info := 'Error generating report: ' || SQLERRM;
        report_result.match_details := '{}';
        report_result.team_members_by_match := '{}';
        report_result.management_members_by_match := '{}';
        
        cost_summary.total_matches := 0;
        cost_summary.total_transportation_cost := 0;
        cost_summary.total_accommodation_cost := 0;
        cost_summary.total_travel_cost := 0;
        cost_summary.average_cost_per_match := 0;
        cost_summary.matches_with_accommodation := 0;
        cost_summary.matches_with_transportation := 0;
        report_result.cost_summary := cost_summary;
        
        RETURN report_result;
END;
$$ LANGUAGE plpgsql;

-- Pomocna funkcija za brzo dobijanje samo sumarnih troskova
CREATE OR REPLACE FUNCTION get_travel_cost_summary(
    p_season_id INTEGER DEFAULT NULL
) RETURNS travel_cost_summary AS $$
DECLARE
    summary_result travel_cost_summary;
BEGIN
    WITH away_matches_summary AS (
        SELECT 
            COUNT(DISTINCT m.id_match) as total_matches,
            SUM(COALESCE(to_offer.price, 0)) as total_transport_cost,
            SUM(COALESCE(ao_offer.price, 0)) as total_accommodation_cost,
            COUNT(CASE WHEN ao_offer.price > 0 THEN 1 END) as matches_with_accommodation,
            COUNT(CASE WHEN to_offer.price > 0 THEN 1 END) as matches_with_transportation
        FROM match m
        INNER JOIN trip tr ON m.id_match = tr.match_id_match
        LEFT JOIN offer to_offer ON (
            tr.id_transportation_offer = to_offer.id_offer 
            AND tr.id_transportation_agency = to_offer.id_agency 
            AND tr.id_transportation_request = to_offer.id_request
        )
        LEFT JOIN offer ao_offer ON (
            tr.id_accommodation_offer = ao_offer.id_offer 
            AND tr.id_accommodation_agency = ao_offer.id_agency 
            AND tr.id_accommodation_request = ao_offer.id_request
        )
        WHERE m.type = 'away'
        AND (p_season_id IS NULL OR m.id_season = p_season_id)
        GROUP BY ()
        HAVING COUNT(DISTINCT m.id_match) > 0
    )
    SELECT 
        total_matches,
        total_transport_cost,
        total_accommodation_cost,
        total_transport_cost + total_accommodation_cost,
        CASE WHEN total_matches > 0 THEN (total_transport_cost + total_accommodation_cost)::NUMERIC / total_matches ELSE 0 END,
        matches_with_accommodation,
        matches_with_transportation
    INTO 
        summary_result.total_matches,
        summary_result.total_transportation_cost,
        summary_result.total_accommodation_cost,
        summary_result.total_travel_cost,
        summary_result.average_cost_per_match,
        summary_result.matches_with_accommodation,
        summary_result.matches_with_transportation
    FROM away_matches_summary;
    
    IF summary_result.total_matches IS NULL THEN
        summary_result.total_matches := 0;
        summary_result.total_transportation_cost := 0;
        summary_result.total_accommodation_cost := 0;
        summary_result.total_travel_cost := 0;
        summary_result.average_cost_per_match := 0;
        summary_result.matches_with_accommodation := 0;
        summary_result.matches_with_transportation := 0;
    END IF;
    
    RETURN summary_result;
END;
$$ LANGUAGE plpgsql;

-- KRAJ IZVESTAJA O TROSKOVIMA PUTOVANJA SANJA RADIC

--------------------------------------------------------------------------
-- SRDJAN ILIC - TRIGGER ZA AUTOMATSKO AŽURIRANJE REZULTATA U MATCH_TRACKING
-- Kada se upiše personal event sa tipom +2p, +3p ili +ft, automatski se ažurira rezultat

CREATE OR REPLACE FUNCTION update_match_score_on_personal_event()
RETURNS TRIGGER AS $$
DECLARE
    points_to_add INTEGER := 0;
    target_match_id INTEGER;
    target_team_id INTEGER;
BEGIN
    -- Određujemo operaciju i uzimamo odgovarajuće vrednosti
    IF TG_OP = 'INSERT' THEN
        target_match_id := NEW.id_match;
        target_team_id := NEW.id_team;
        
        -- Proveravamo da li je event tip koji donosi poene
        CASE NEW.type
            WHEN '+2p' THEN points_to_add := 2;
            WHEN '+3p' THEN points_to_add := 3;
            WHEN '+ft' THEN points_to_add := 1;
            ELSE points_to_add := 0;
        END CASE;
        
    ELSIF TG_OP = 'DELETE' THEN
        target_match_id := OLD.id_match;
        target_team_id := OLD.id_team;
        
        -- Pri brisanju, oduzimamo poene (negativni points_to_add)
        CASE OLD.type
            WHEN '+2p' THEN points_to_add := -2;
            WHEN '+3p' THEN points_to_add := -3;
            WHEN '+ft' THEN points_to_add := -1;
            ELSE points_to_add := 0;
        END CASE;
    END IF;
    
    -- Ako event utiče na poene, ažuriramo rezultat u match_tracking
    IF points_to_add != 0 THEN
        UPDATE match_tracking 
        SET 
            our_points = CASE 
                WHEN target_team_id = 1 THEN GREATEST(0, COALESCE(our_points, 0) + points_to_add)
                ELSE COALESCE(our_points, 0)
            END,
            opponent_points = CASE 
                WHEN target_team_id != 1 THEN GREATEST(0, COALESCE(opponent_points, 0) + points_to_add)
                ELSE COALESCE(opponent_points, 0)
            END,
            last_update_time = CURRENT_TIMESTAMP
        WHERE id_match = target_match_id;
    END IF;
    
    -- Vraćamo odgovarajući red u zavisnosti od operacije
    IF TG_OP = 'INSERT' THEN
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
        RETURN OLD;
    END IF;
    
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

-- Kreiranje triggera koji poziva funkciju kada se upisuje ili briše personal event
CREATE TRIGGER trigger_update_match_score_on_personal_event
    AFTER INSERT OR DELETE ON personal_event
    FOR EACH ROW
    EXECUTE FUNCTION update_match_score_on_personal_event();

-- KRAJ TRIGGERA SRDJAN ILIC

--------------------------------------------------------------------------
-- SRDJAN ILIC - KOMPLEKSNA FUNKCIJA ZA EFIKASNOST IGRAČA NA UTAKMICI
-- Računa detaljnu efikasnost igrača na osnovu svih njegovih personal_event zapisa

CREATE TYPE player_efficiency_stats AS (
    player_id INTEGER,
    team_id INTEGER,
    match_id INTEGER,
    player_name VARCHAR(255),
    match_name VARCHAR(255),
    total_points INTEGER,
    total_assists INTEGER,
    total_rebounds INTEGER,
    total_steals INTEGER,
    total_blocks INTEGER,
    total_fouls INTEGER,
    shooting_2p_made INTEGER,
    shooting_2p_attempted INTEGER,
    shooting_2p_percentage NUMERIC(5,2),
    shooting_3p_made INTEGER,
    shooting_3p_attempted INTEGER,
    shooting_3p_percentage NUMERIC(5,2),
    free_throws_made INTEGER,
    free_throws_attempted INTEGER,
    free_throw_percentage NUMERIC(5,2),
    offensive_rebounds INTEGER,
    defensive_rebounds INTEGER,
    substitutions_count INTEGER,
    efficiency_rating NUMERIC(8,2),
    performance_grade VARCHAR(10),
    minutes_played INTEGER,
    plus_minus_rating NUMERIC(6,2)
);

CREATE OR REPLACE FUNCTION calculate_player_match_efficiency(
    p_player_id INTEGER,
    p_team_id INTEGER,
    p_match_id INTEGER
) RETURNS player_efficiency_stats AS $$
DECLARE
    result player_efficiency_stats;
    
    -- Osnovne statistike
    v_points_2p INTEGER := 0;
    v_points_3p INTEGER := 0;
    v_points_ft INTEGER := 0;
    v_assists INTEGER := 0;
    v_rebounds_off INTEGER := 0;
    v_rebounds_def INTEGER := 0;
    v_steals INTEGER := 0;
    v_blocks INTEGER := 0;
    v_fouls INTEGER := 0;
    
    -- Šuterske statistike
    v_2p_made INTEGER := 0;
    v_2p_attempted INTEGER := 0;
    v_3p_made INTEGER := 0;
    v_3p_attempted INTEGER := 0;
    v_ft_made INTEGER := 0;
    v_ft_attempted INTEGER := 0;
    
    -- Dodatne statistike
    v_substitutions INTEGER := 0;
    v_sub_in_time INTEGER := 0;
    v_sub_out_time INTEGER := 0;
    v_minutes_played INTEGER := 0;
    v_is_currently_playing BOOLEAN := FALSE;
    v_is_starting_lineup BOOLEAN := FALSE;
    
    -- Za plus/minus rating
    v_team_points_when_playing INTEGER := 0;
    v_opponent_points_when_playing INTEGER := 0;
    
    -- Pomoćne varijable
    v_player_name VARCHAR(255);
    v_match_name VARCHAR(255);
    v_efficiency NUMERIC(8,2);
    v_grade VARCHAR(10);
    v_team_id INTEGER;
    
    -- Cursor za prolaz kroz sve evente igrača u specificnom timu
    event_cursor CURSOR FOR
        SELECT pe.type, pe.period_time, pe.period, pe.creation_time
        FROM personal_event pe
        WHERE pe.id_player = p_player_id 
          AND pe.id_team = p_team_id
          AND pe.id_match = p_match_id
        ORDER BY pe.creation_time;
        
BEGIN
    -- Dohvatanje osnovnih informacija i provera da li je u startnoj postavi
    SELECT CONCAT(p.name, ' ', p.surname), m.name, COALESCE(tmm.starting_lineup, FALSE)
    INTO v_player_name, v_match_name, v_is_starting_lineup
    FROM player p
    CROSS JOIN match m
    LEFT JOIN team_member_match tmm ON tmm.id_player = p.id_player 
                                    AND tmm.id_team = p_team_id 
                                    AND tmm.id_match = m.id_match
    WHERE p.id_player = p_player_id 
      AND m.id_match = p_match_id
    LIMIT 1;
    
    -- Postavi team_id za rezultat
    v_team_id := p_team_id;
    
    -- Ako nema podataka, vrati prazan rezultat
    IF v_player_name IS NULL THEN
        result.player_name := 'Player not found';
        result.match_name := 'Match not found';
        RETURN result;
    END IF;
    
    -- Ako je u startnoj postavi, automatski postaviti da igra od početka
    IF v_is_starting_lineup THEN
        v_is_currently_playing := TRUE;
        v_sub_in_time := 0; -- Počinje od početka utakmice
    END IF;
    
    -- Prolaz kroz sve evente i računanje statistika
    FOR event_rec IN event_cursor LOOP
        CASE event_rec.type
            -- Poeni
            WHEN '+2p' THEN
                v_points_2p := v_points_2p + 2;
                v_2p_made := v_2p_made + 1;
            WHEN '+3p' THEN
                v_points_3p := v_points_3p + 3;
                v_3p_made := v_3p_made + 1;
            WHEN '+ft' THEN
                v_points_ft := v_points_ft + 1;
                v_ft_made := v_ft_made + 1;
                
            -- Promašaji
            WHEN '2p' THEN
                v_2p_attempted := v_2p_attempted + 1;
            WHEN '3p' THEN
                v_3p_attempted := v_3p_attempted + 1;
            WHEN 'ft' THEN
                v_ft_attempted := v_ft_attempted + 1;
                
            -- Ostale statistike
            WHEN 'assist' THEN
                v_assists := v_assists + 1;
            WHEN 'reb of' THEN
                v_rebounds_off := v_rebounds_off + 1;
            WHEN 'reb def' THEN
                v_rebounds_def := v_rebounds_def + 1;
            WHEN 'steal' THEN
                v_steals := v_steals + 1;
            WHEN 'block' THEN
                v_blocks := v_blocks + 1;
            WHEN 'foul' THEN
                v_fouls := v_fouls + 1;
                
            -- Izmene
            WHEN 'substitution in' THEN
                v_substitutions := v_substitutions + 1;
                v_sub_in_time := COALESCE(event_rec.period_time, 0);
                v_is_currently_playing := TRUE;
            WHEN 'substitution out' THEN
                v_sub_out_time := COALESCE(event_rec.period_time, 0);
                v_is_currently_playing := FALSE;
                -- Dodaj vreme igranja za ovaj segment
                IF v_sub_in_time >= 0 THEN  -- >=0 jer startni igrači počinju od 0
                    v_minutes_played := v_minutes_played + (v_sub_out_time - v_sub_in_time);
                END IF;
                -- Reset sub_in_time za sledeći ulazak
                v_sub_in_time := -1;
        END CASE;
    END LOOP;
    
    -- Ako je igrač još uvek u igri na kraju (nije izašao), dodaj vreme do kraja utakmice
    -- Pretpostavljamo da je utakmica 4 perioda po 10 minuta = 2400000 milisekundi
    IF v_is_currently_playing AND v_sub_in_time >= 0 THEN
        v_minutes_played := v_minutes_played + (2400000 - v_sub_in_time); -- 40 minuta = 2400000ms
    END IF;
    
    -- Dodaj ukupne pokušaje za pogođene šuteve
    v_2p_attempted := v_2p_attempted + v_2p_made;
    v_3p_attempted := v_3p_attempted + v_3p_made;
    v_ft_attempted := v_ft_attempted + v_ft_made;
    
    -- Konvertuj milisekunde u minute
    v_minutes_played := v_minutes_played / 60000;
    
    -- Izračunavanje efikasnosti po NBA formuli:
    -- EFF = (Points + Rebounds + Assists + Steals + Blocks) - (FG Missed + FT Missed + Turnovers)
    -- Pošto nemamo turnovers, koristićemo faule kao aproksimaciju
    v_efficiency := (v_points_2p + v_points_3p + v_points_ft) + 
                   (v_rebounds_off + v_rebounds_def) + 
                   v_assists + v_steals + v_blocks - 
                   ((v_2p_attempted - v_2p_made) + (v_3p_attempted - v_3p_made) + 
                    (v_ft_attempted - v_ft_made) + v_fouls);
    
    -- Izračunavanje efikasnosti BEZ normalizacije po vremenu
    -- Efikasnost je apsolutna vrednost za utakmicu, ne zavisi od vremena igranja
    
    -- Određivanje ocene na osnovu efikasnosti
    IF v_efficiency >= 30 THEN
        v_grade := 'A+';
    ELSIF v_efficiency >= 25 THEN
        v_grade := 'A';
    ELSIF v_efficiency >= 20 THEN
        v_grade := 'B+';
    ELSIF v_efficiency >= 15 THEN
        v_grade := 'B';
    ELSIF v_efficiency >= 10 THEN
        v_grade := 'C+';
    ELSIF v_efficiency >= 5 THEN
        v_grade := 'C';
    ELSE
        v_grade := 'D';
    END IF;
    
    -- Računanje plus/minus (aproksimacija - razlika poena dok je igrao)
    -- Ovo je pojednostavljeno jer bi trebalo da pratimo tačno kad je igrao
    SELECT 
        COALESCE(mt.our_points, 0) - COALESCE(mt.opponent_points, 0)
    INTO v_team_points_when_playing
    FROM match_tracking mt
    WHERE mt.id_match = p_match_id;
    
    -- Popunjavanje rezultata
    result.player_id := p_player_id;
    result.team_id := p_team_id;
    result.match_id := p_match_id;
    result.player_name := v_player_name;
    result.match_name := v_match_name;
    result.total_points := v_points_2p + v_points_3p + v_points_ft;
    result.total_assists := v_assists;
    result.total_rebounds := v_rebounds_off + v_rebounds_def;
    result.total_steals := v_steals;
    result.total_blocks := v_blocks;
    result.total_fouls := v_fouls;
    result.shooting_2p_made := v_2p_made;
    result.shooting_2p_attempted := v_2p_attempted;
    result.shooting_2p_percentage := CASE 
        WHEN v_2p_attempted > 0 THEN ROUND((v_2p_made::NUMERIC / v_2p_attempted) * 100, 2)
        ELSE 0 
    END;
    result.shooting_3p_made := v_3p_made;
    result.shooting_3p_attempted := v_3p_attempted;
    result.shooting_3p_percentage := CASE 
        WHEN v_3p_attempted > 0 THEN ROUND((v_3p_made::NUMERIC / v_3p_attempted) * 100, 2)
        ELSE 0 
    END;
    result.free_throws_made := v_ft_made;
    result.free_throws_attempted := v_ft_attempted;
    result.free_throw_percentage := CASE 
        WHEN v_ft_attempted > 0 THEN ROUND((v_ft_made::NUMERIC / v_ft_attempted) * 100, 2)
        ELSE 0 
    END;
    result.offensive_rebounds := v_rebounds_off;
    result.defensive_rebounds := v_rebounds_def;
    result.substitutions_count := v_substitutions;
    result.efficiency_rating := ROUND(v_efficiency, 2);
    result.performance_grade := v_grade;
    result.minutes_played := v_minutes_played;
    result.plus_minus_rating := ROUND(v_team_points_when_playing::NUMERIC, 2);
    
    RETURN result;
    
EXCEPTION
    WHEN OTHERS THEN
        -- U slučaju greške, vrati osnovne informacije
        result.player_id := p_player_id;
        result.team_id := p_team_id;
        result.match_id := p_match_id;
        result.player_name := COALESCE(v_player_name, 'Unknown Player');
        result.match_name := COALESCE(v_match_name, 'Unknown Match');
        result.efficiency_rating := 0;
        result.performance_grade := 'N/A';
        RETURN result;
END;
$$ LANGUAGE plpgsql;

-- FUNKCIJA ZA DOBIJANJE STATISTIKA SVIH IGRAČA NA UTAKMICI
CREATE OR REPLACE FUNCTION get_match_player_statistics(
    p_match_id INTEGER
) RETURNS SETOF player_efficiency_stats AS $$
DECLARE
    player_rec RECORD;
    efficiency_result player_efficiency_stats;
BEGIN
    -- Prolazimo kroz sve igrače oba tima na datoj utakmici
    FOR player_rec IN
        SELECT DISTINCT 
            tmm.id_player,
            tmm.id_team,
            p.name,
            p.surname
        FROM team_member_match tmm
        JOIN player p ON p.id_player = tmm.id_player
        WHERE tmm.id_match = p_match_id
        ORDER BY tmm.id_team, p.name, p.surname
    LOOP
        -- Pozivamo funkciju za računanje efikasnosti svakog igrača
        SELECT * INTO efficiency_result 
        FROM calculate_player_match_efficiency(
            player_rec.id_player,
            player_rec.id_team,
            p_match_id
        );
        
        -- Vraćamo rezultat
        RETURN NEXT efficiency_result;
    END LOOP;
    
    RETURN;
    
EXCEPTION
    WHEN OTHERS THEN
        -- U slučaju greške, vraćamo grešku kao rezultat
        efficiency_result.player_name := 'Error occurred';
        efficiency_result.match_name := SQLERRM;
        efficiency_result.efficiency_rating := 0;
        efficiency_result.performance_grade := 'ERROR';
        RETURN NEXT efficiency_result;
        RETURN;
END;
$$ LANGUAGE plpgsql;

--------------------------------------------------------------------------
-- SRDJAN ILIC - KOMPLEKSAN IZVEŠTAJ SA GENERALNIM PODACIMA I TEAM STATISTIKAMA
-- Koristi složene tipove, kursore, WITH klauzule i agregacione operacije

-- Složeni tip za team statistike
CREATE TYPE team_match_stats AS (
    team_id INTEGER,
    team_name VARCHAR(255),
    total_points INTEGER,
    total_field_goals_made INTEGER,
    total_field_goals_attempted INTEGER,
    field_goal_percentage NUMERIC(5,2),
    total_2p_made INTEGER,
    total_2p_attempted INTEGER,
    two_point_percentage NUMERIC(5,2),
    total_3p_made INTEGER,
    total_3p_attempted INTEGER,
    three_point_percentage NUMERIC(5,2),
    total_free_throws_made INTEGER,
    total_free_throws_attempted INTEGER,
    free_throw_percentage NUMERIC(5,2),
    total_rebounds INTEGER,
    total_offensive_rebounds INTEGER,
    total_defensive_rebounds INTEGER,
    total_assists INTEGER,
    total_steals INTEGER,
    total_blocks INTEGER,
    total_fouls INTEGER,
    team_efficiency_rating NUMERIC(8,2),
    active_players_count INTEGER,
    substitutions_count INTEGER,
    avg_player_efficiency NUMERIC(6,2),
    best_player_name VARCHAR(255),
    best_player_efficiency NUMERIC(8,2)
);

-- Složeni tip za generalne podatke o utakmici
CREATE TYPE match_general_info AS (
    match_id INTEGER,
    match_name VARCHAR(255),
    scheduled_at TIMESTAMP,
    hall VARCHAR(255),
    city VARCHAR(255),
    state VARCHAR(50),
    duration_minutes INTEGER,
    total_events_count INTEGER,
    highest_individual_score INTEGER,
    lowest_individual_score INTEGER,
    total_substitutions INTEGER,
    total_fouls INTEGER,
    our_team_id INTEGER,
    opponent_team_id INTEGER,
    final_score_our INTEGER,
    final_score_opponent INTEGER
);

-- Glavni složeni tip koji objedinjuje sve
CREATE TYPE complete_match_report AS (
    general_info match_general_info,
    our_team_stats team_match_stats,
    opponent_team_stats team_match_stats,
    our_players_stats player_efficiency_stats[],
    opponent_players_stats player_efficiency_stats[]
);

-- Funkcija za računanje team statistika sa kursorom i agregacije
CREATE OR REPLACE FUNCTION calculate_team_match_stats(
    p_team_id INTEGER,
    p_match_id INTEGER
) RETURNS team_match_stats AS $$
DECLARE
    result team_match_stats;
    
    -- Varijable za kursor
    player_cursor CURSOR FOR
        SELECT tmm.id_player, p.name, p.surname
        FROM team_member_match tmm
        JOIN player p ON p.id_player = tmm.id_player
        WHERE tmm.id_team = p_team_id AND tmm.id_match = p_match_id;
    
    player_rec RECORD;
    player_stats player_efficiency_stats;
    
    -- Agregacione varijable
    v_total_points INTEGER := 0;
    v_total_2p_made INTEGER := 0;
    v_total_2p_attempted INTEGER := 0;
    v_total_3p_made INTEGER := 0;
    v_total_3p_attempted INTEGER := 0;
    v_total_ft_made INTEGER := 0;
    v_total_ft_attempted INTEGER := 0;
    v_total_rebounds INTEGER := 0;
    v_total_off_rebounds INTEGER := 0;
    v_total_def_rebounds INTEGER := 0;
    v_total_assists INTEGER := 0;
    v_total_steals INTEGER := 0;
    v_total_blocks INTEGER := 0;
    v_total_fouls INTEGER := 0;
    v_total_substitutions INTEGER := 0;
    v_active_players INTEGER := 0;
    v_team_efficiency NUMERIC(8,2) := 0;
    v_avg_efficiency NUMERIC(6,2) := 0;
    v_best_player_name VARCHAR(255) := '';
    v_best_efficiency NUMERIC(8,2) := 0;
    v_team_name VARCHAR(255);
    
BEGIN
    -- Dobijanje naziva tima
    SELECT name INTO v_team_name FROM team WHERE id_team = p_team_id;
    
    -- Korišćenje kursora za prolaz kroz sve igrače tima
    FOR player_rec IN player_cursor LOOP
        -- Pozivanje funkcije za statistike igrača
        SELECT * INTO player_stats 
        FROM calculate_player_match_efficiency(player_rec.id_player, p_team_id, p_match_id);
        
        -- Agregiranje statistika
        v_total_points := v_total_points + COALESCE(player_stats.total_points, 0);
        v_total_2p_made := v_total_2p_made + COALESCE(player_stats.shooting_2p_made, 0);
        v_total_2p_attempted := v_total_2p_attempted + COALESCE(player_stats.shooting_2p_attempted, 0);
        v_total_3p_made := v_total_3p_made + COALESCE(player_stats.shooting_3p_made, 0);
        v_total_3p_attempted := v_total_3p_attempted + COALESCE(player_stats.shooting_3p_attempted, 0);
        v_total_ft_made := v_total_ft_made + COALESCE(player_stats.free_throws_made, 0);
        v_total_ft_attempted := v_total_ft_attempted + COALESCE(player_stats.free_throws_attempted, 0);
        v_total_rebounds := v_total_rebounds + COALESCE(player_stats.total_rebounds, 0);
        v_total_off_rebounds := v_total_off_rebounds + COALESCE(player_stats.offensive_rebounds, 0);
        v_total_def_rebounds := v_total_def_rebounds + COALESCE(player_stats.defensive_rebounds, 0);
        v_total_assists := v_total_assists + COALESCE(player_stats.total_assists, 0);
        v_total_steals := v_total_steals + COALESCE(player_stats.total_steals, 0);
        v_total_blocks := v_total_blocks + COALESCE(player_stats.total_blocks, 0);
        v_total_fouls := v_total_fouls + COALESCE(player_stats.total_fouls, 0);
        v_total_substitutions := v_total_substitutions + COALESCE(player_stats.substitutions_count, 0);
        
        v_active_players := v_active_players + 1;
        
        -- Pronalaženje najboljeg igrača
        IF COALESCE(player_stats.efficiency_rating, 0) > v_best_efficiency THEN
            v_best_efficiency := COALESCE(player_stats.efficiency_rating, 0);
            v_best_player_name := COALESCE(player_stats.player_name, 'Unknown');
        END IF;
        
        v_team_efficiency := v_team_efficiency + COALESCE(player_stats.efficiency_rating, 0);
    END LOOP;
    
    -- Računanje proseka
    IF v_active_players > 0 THEN
        v_avg_efficiency := v_team_efficiency / v_active_players;
    END IF;
    
    -- Popunjavanje rezultata
    result.team_id := p_team_id;
    result.team_name := COALESCE(v_team_name, 'Unknown Team');
    result.total_points := v_total_points;
    result.total_field_goals_made := v_total_2p_made + v_total_3p_made;
    result.total_field_goals_attempted := v_total_2p_attempted + v_total_3p_attempted;
    result.field_goal_percentage := CASE 
        WHEN (v_total_2p_attempted + v_total_3p_attempted) > 0 
        THEN ROUND(((v_total_2p_made + v_total_3p_made)::NUMERIC / (v_total_2p_attempted + v_total_3p_attempted)) * 100, 2)
        ELSE 0 
    END;
    result.total_2p_made := v_total_2p_made;
    result.total_2p_attempted := v_total_2p_attempted;
    result.two_point_percentage := CASE 
        WHEN v_total_2p_attempted > 0 
        THEN ROUND((v_total_2p_made::NUMERIC / v_total_2p_attempted) * 100, 2)
        ELSE 0 
    END;
    result.total_3p_made := v_total_3p_made;
    result.total_3p_attempted := v_total_3p_attempted;
    result.three_point_percentage := CASE 
        WHEN v_total_3p_attempted > 0 
        THEN ROUND((v_total_3p_made::NUMERIC / v_total_3p_attempted) * 100, 2)
        ELSE 0 
    END;
    result.total_free_throws_made := v_total_ft_made;
    result.total_free_throws_attempted := v_total_ft_attempted;
    result.free_throw_percentage := CASE 
        WHEN v_total_ft_attempted > 0 
        THEN ROUND((v_total_ft_made::NUMERIC / v_total_ft_attempted) * 100, 2)
        ELSE 0 
    END;
    result.total_rebounds := v_total_rebounds;
    result.total_offensive_rebounds := v_total_off_rebounds;
    result.total_defensive_rebounds := v_total_def_rebounds;
    result.total_assists := v_total_assists;
    result.total_steals := v_total_steals;
    result.total_blocks := v_total_blocks;
    result.total_fouls := v_total_fouls;
    result.team_efficiency_rating := ROUND(v_team_efficiency, 2);
    result.active_players_count := v_active_players;
    result.substitutions_count := v_total_substitutions;
    result.avg_player_efficiency := ROUND(v_avg_efficiency, 2);
    result.best_player_name := v_best_player_name;
    result.best_player_efficiency := ROUND(v_best_efficiency, 2);
    
    RETURN result;
    
EXCEPTION
    WHEN OTHERS THEN
        result.team_id := p_team_id;
        result.team_name := 'Error';
        RETURN result;
END;
$$ LANGUAGE plpgsql;

-- Glavna funkcija za kompletan izveštaj sa WITH klauzulom i složenim upitima
CREATE OR REPLACE FUNCTION generate_complete_match_report(
    p_match_id INTEGER
) RETURNS complete_match_report AS $$
DECLARE
    result complete_match_report;
    v_our_team_id INTEGER;
    v_opponent_team_id INTEGER;
    v_match_info match_general_info;
    v_our_stats team_match_stats;
    v_opponent_stats team_match_stats;
    v_our_players player_efficiency_stats[];
    v_opponent_players player_efficiency_stats[];
    
    -- Za iteraciju kroz igrače
    player_rec player_efficiency_stats;
    
BEGIN
    -- Korišćenje složenih upita za osnovne podatke
    SELECT 
        m.id_match,
        m.name,
        m.scheduled_at,
        m.hall,
        m.city,
        m.state,
        COALESCE(mt.period_duration * 4 / 60000, 40), -- Convert from milliseconds to minutes, default 40
        (SELECT COUNT(*) FROM personal_event pe WHERE pe.id_match = m.id_match),
        (SELECT COUNT(*) 
         FROM personal_event pe 
         WHERE pe.id_match = m.id_match 
           AND pe.type IN ('substitution in')),
        (SELECT COUNT(*) 
         FROM personal_event pe 
         WHERE pe.id_match = m.id_match 
           AND pe.type = 'foul'),
        COALESCE(mt.our_points, 0),
        COALESCE(mt.opponent_points, 0)
        
    INTO v_match_info.match_id, v_match_info.match_name, v_match_info.scheduled_at,
         v_match_info.hall, v_match_info.city, v_match_info.state,
         v_match_info.duration_minutes, v_match_info.total_events_count,
         v_match_info.total_substitutions, v_match_info.total_fouls,
         v_match_info.final_score_our, v_match_info.final_score_opponent
    FROM match m
    LEFT JOIN match_tracking mt ON mt.id_match = m.id_match
    WHERE m.id_match = p_match_id
    LIMIT 1;
    
    -- Određivanje našeg i protivničkog tima
    -- Naš tim je uvek tim sa ID = 1 (KK Partizan), ostali su protivnici
    SELECT 
        CASE WHEN EXISTS(SELECT 1 FROM team_member_match WHERE id_match = p_match_id AND id_team = 1) 
             THEN 1 
             ELSE (SELECT MIN(id_team) FROM team_member_match WHERE id_match = p_match_id) 
        END as our_team,
        CASE WHEN EXISTS(SELECT 1 FROM team_member_match WHERE id_match = p_match_id AND id_team = 1) 
             THEN (SELECT MIN(id_team) FROM team_member_match WHERE id_match = p_match_id AND id_team != 1)
             ELSE (SELECT MAX(id_team) FROM team_member_match WHERE id_match = p_match_id) 
        END as opponent_team
    INTO v_our_team_id, v_opponent_team_id;
    
    -- Računanje statistika za oba tima
    SELECT * INTO v_our_stats FROM calculate_team_match_stats(v_our_team_id, p_match_id);
    SELECT * INTO v_opponent_stats FROM calculate_team_match_stats(v_opponent_team_id, p_match_id);
    
    -- Dodatne informacije za match_general_info
    v_match_info.our_team_id := v_our_team_id;
    v_match_info.opponent_team_id := v_opponent_team_id;
    
    -- Dobijanje svih statistika igrača i podela po timovima
    v_our_players := ARRAY[]::player_efficiency_stats[];
    v_opponent_players := ARRAY[]::player_efficiency_stats[];
    
    -- Iteracija kroz sve igrače i podela po timovima
    FOR player_rec IN 
        SELECT * FROM get_match_player_statistics(p_match_id)
    LOOP
        -- Dodavanje igrača u odgovarajući niz na osnovu team_id
        IF player_rec.team_id = v_our_team_id THEN
            v_our_players := array_append(v_our_players, player_rec);
        ELSE
            v_opponent_players := array_append(v_opponent_players, player_rec);
        END IF;
    END LOOP;
    
    -- Određivanje najviše/najniže individualne efikasnosti koristeći WITH klauzulu
    WITH player_efficiency_summary AS (
        SELECT 
            efficiency_rating
        FROM get_match_player_statistics(p_match_id)
        WHERE efficiency_rating IS NOT NULL
    )
    SELECT 
        COALESCE(MAX(efficiency_rating), 0),
        COALESCE(MIN(efficiency_rating), 0)
    INTO v_match_info.highest_individual_score, v_match_info.lowest_individual_score
    FROM player_efficiency_summary;
    
    -- Popunjavanje finalnog rezultata
    result.general_info := v_match_info;
    result.our_team_stats := v_our_stats;
    result.opponent_team_stats := v_opponent_stats;
    result.our_players_stats := v_our_players;
    result.opponent_players_stats := v_opponent_players;
    
    RETURN result;
    
EXCEPTION
    WHEN OTHERS THEN
        -- U slučaju greške
        result.general_info.match_id := p_match_id;
        result.general_info.match_name := 'Error occurred';
        RETURN result;
END;
$$ LANGUAGE plpgsql;

-- KRAJ FUNKCIJA SRDJAN ILIC