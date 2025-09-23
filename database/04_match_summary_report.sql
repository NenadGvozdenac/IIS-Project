-- Brisanje postojećih tipova i funkcija
DROP TYPE IF EXISTS match_summary_row CASCADE;
DROP FUNCTION IF EXISTS generate_match_summary_report();

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

-- SELECT * FROM generate_match_summary_report();