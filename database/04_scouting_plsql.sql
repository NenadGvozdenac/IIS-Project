-- I. Indexes for Scouting Performance

-- Index 1: Optimizes session retrieval for a specific player, optionally filtered by time or type
CREATE INDEX IF NOT EXISTS idx_session_player_time_type ON session (id_player, start_time DESC, id_session_type);

-- Index 2: Optimizes join between session and its metrics
CREATE INDEX IF NOT EXISTS idx_session_metrics_session_metric ON session_metrics (id_session, id_metrics);

-- Index 3: Speeds up filtering of metrics for season averages
CREATE INDEX IF NOT EXISTS idx_metrics_type_permanent ON metrics (id_metric_type, is_permanent);

-- Index 4: Efficiently retrieves a player's physical history
CREATE INDEX IF NOT EXISTS idx_physical_metrics_player_date ON physical_metrics (id_player, date_of_measurement DESC);

-- Index 5: For quick team membership check
CREATE INDEX IF NOT EXISTS idx_team_member_player_team ON team_member (id_player, id_team);

-- Function to get player metric averages for a season
-- This function returns all quantitative metrics with their average values for a specific player and season
CREATE OR REPLACE FUNCTION get_player_season_metric_averages(
    p_player_id INTEGER,
    p_season_id INTEGER,
    p_session_type_filter VARCHAR DEFAULT NULL -- NULL means all session types
)
RETURNS TABLE (
    metric_id INTEGER,
    metric_name VARCHAR,
    metric_weight INTEGER,
    average_value DECIMAL(10,2),
    session_count INTEGER
) AS $$
BEGIN
    RETURN QUERY
    WITH season_metrics AS (
        -- Get all metrics for the season (permanent + season-specific)
        SELECT DISTINCT 
            m.id_metrics,
            m.name,
            m.metric_weight,
            m.id_metric_type
        FROM metrics m
        WHERE (m.is_permanent = 1 OR m.id_metrics IN (
            SELECT sm.id_metrics 
            FROM season_metrics sm 
            WHERE sm.id_season = p_season_id
        ))
        AND m.id_metric_type = 1 -- Only quantitative metrics
    ),
    player_sessions AS (
        -- Get all sessions for the player
        SELECT s.id_session, s.id_session_type
        FROM session s
        WHERE s.id_player = p_player_id
        AND (p_session_type_filter IS NULL OR 
             s.id_session_type IN (
                 SELECT st.id_type 
                 FROM session_type st 
                 WHERE st.type = p_session_type_filter
             ))
    ),
    metric_values AS (
        -- Get metric values from sessions
        SELECT 
            sm.id_metrics,
            sm.value,
            ps.id_session
        FROM session_metrics sm
        INNER JOIN player_sessions ps ON sm.id_session = ps.id_session
        WHERE sm.value ~ '^[0-9]+\.?[0-9]*$' -- Only numeric values
    )
    SELECT 
        sm.id_metrics::INTEGER,
        sm.name::VARCHAR,
        sm.metric_weight::INTEGER,
        ROUND(AVG(CAST(mv.value AS DECIMAL)), 2) as average_value,
        COUNT(DISTINCT mv.id_session)::INTEGER as session_count
    FROM season_metrics sm
    LEFT JOIN metric_values mv ON sm.id_metrics = mv.id_metrics
    GROUP BY sm.id_metrics, sm.name, sm.metric_weight
    HAVING COUNT(mv.value) > 0 -- Only return metrics that have values
    ORDER BY sm.metric_weight DESC, sm.name;
END;
$$ LANGUAGE plpgsql;

-- Function to get all sessions for a player with filters
CREATE OR REPLACE FUNCTION get_player_sessions(
    p_player_id INTEGER,
    p_season_id INTEGER DEFAULT NULL,
    p_status_filter VARCHAR DEFAULT NULL,
    p_date_from DATE DEFAULT NULL,
    p_date_to DATE DEFAULT NULL
)
RETURNS TABLE (
    session_id INTEGER,
    start_time DATE,
    end_time DATE,
    session_status VARCHAR,
    session_type VARCHAR,
    scout_name VARCHAR,
    scout_surname VARCHAR
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        s.id_session::INTEGER,
        s.start_time::DATE,
        s.end_time::DATE,
        ss.status::VARCHAR,
        st.type::VARCHAR,
        u.name::VARCHAR,
        u.surname::VARCHAR
    FROM session s
    INNER JOIN session_status ss ON s.id_session_status = ss.id_status
    INNER JOIN session_type st ON s.id_session_type = st.id_type
    INNER JOIN users u ON s.id_user = u.id_user
    WHERE s.id_player = p_player_id
    AND (p_status_filter IS NULL OR ss.status = p_status_filter)
    AND (p_date_from IS NULL OR s.start_time >= p_date_from)
    AND (p_date_to IS NULL OR s.start_time <= p_date_to)
    ORDER BY s.start_time DESC;
END;
$$ LANGUAGE plpgsql;

-- II. Triggers for Scouting Automation

-- FUNCTION 1: Auto-update player table on new physical metrics
CREATE OR REPLACE FUNCTION update_player_latest_physical_metrics()
RETURNS TRIGGER AS $$
BEGIN
    -- Update the player's main weight and height columns
    UPDATE player
    SET 
        weight = NEW.weight,
        height = NEW.height
    WHERE id_player = NEW.id_player;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- TRIGGER 1: Execute after inserting a new physical_metrics record
CREATE TRIGGER trigger_update_player_latest_physical_metrics
AFTER INSERT ON physical_metrics
FOR EACH ROW
EXECUTE FUNCTION update_player_latest_physical_metrics();

-- =====================================================================


-- FUNCTION 2: Validate quantitative metric values
CREATE OR REPLACE FUNCTION validate_quantitative_metric_value()
RETURNS TRIGGER AS $$
DECLARE
    v_metric_type_id INTEGER;
BEGIN
    -- Get the metric type for the current metric ID
    SELECT id_metric_type INTO v_metric_type_id
    FROM metrics
    WHERE id_metrics = NEW.id_metrics;

    -- If the metric is quantitative (assuming ID 1 for quantitative)
    IF v_metric_type_id = 1 THEN
        -- Check if the value is a valid number (integer or decimal)
        IF NEW.value !~ '^[0-9]+(\.[0-9]+)?$' THEN
            RAISE EXCEPTION 'Quantitative metric "%" must have a valid numeric value. Found: "%"', 
                (SELECT name FROM metrics WHERE id_metrics = NEW.id_metrics), NEW.value;
        END IF;
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- TRIGGER 2: Execute before inserting or updating a session_metrics record
CREATE TRIGGER trigger_validate_quantitative_metric_value
BEFORE INSERT OR UPDATE ON session_metrics
FOR EACH ROW
EXECUTE FUNCTION validate_quantitative_metric_value();


-- =========================================================================

-- REPORT

-- TYPE 1: Individual metric detail
CREATE TYPE player_metric_detail AS (
    metric_name VARCHAR(255),
    metric_weight INTEGER,
    average_value DECIMAL(10,2)
);

-- TYPE 2: Full scouting scorecard report
CREATE TYPE player_scouting_report AS (
    report_date TIMESTAMP WITH TIME ZONE,
    player_id INTEGER,
    player_full_name VARCHAR(255),
    position_name VARCHAR(20),
    nationality_name VARCHAR(255),
    latest_height INTEGER,
    latest_weight INTEGER,
    latest_jump_date DATE,
    latest_vertical_jump INTEGER,
    total_sessions_analyzed INTEGER,
    total_scouting_score NUMERIC(8,3),
    normalized_score NUMERIC(5,2),
    metric_breakdown player_metric_detail[]
);


-- FUNCTION 3: Generate Player Scouting Summary Scorecard
CREATE OR REPLACE FUNCTION generate_player_scouting_summary(
    p_player_id INTEGER,
    p_season_id INTEGER,
    p_normalization_max_score NUMERIC DEFAULT 100.0
)
RETURNS player_scouting_report AS $$
DECLARE
    report_result player_scouting_report;
    v_max_possible_score NUMERIC := 0;
    v_total_score NUMERIC := 0;
    v_metric_record RECORD;
    v_metric_detail player_metric_detail;
    v_metric_breakdown player_metric_detail[] := '{}';
    v_latest_jump_data RECORD;
BEGIN
    report_result.report_date := NOW();
    report_result.player_id := p_player_id;

    -- Calculate max possible score using reasonable assumptions
    WITH max_score_calc AS (
        SELECT SUM(100 * m.metric_weight) as total_weight  -- Assume max metric value is 100
        FROM metrics m
        WHERE m.id_metric_type = 1 -- Quantitative metrics
        AND (m.is_permanent = 1 OR m.id_metrics IN (
            SELECT sm.id_metrics 
            FROM season_metrics sm 
            WHERE sm.id_season = p_season_id
        ))
    )
    SELECT COALESCE(total_weight, 1) INTO v_max_possible_score FROM max_score_calc;
    
    -- Get player and physical info
    SELECT
        CONCAT(p.name, ' ', p.surname),
        pos.name,
        n.state,
        p.height,
        p.weight
    INTO
        report_result.player_full_name,
        report_result.position_name,
        report_result.nationality_name,
        report_result.latest_height,
        report_result.latest_weight
    FROM player p
    JOIN position pos ON p.id_position = pos.id_position
    JOIN nationality n ON p.id_nationality = n.id_nationality
    WHERE p.id_player = p_player_id;

    -- Get latest vertical jump data
    SELECT 
        date_of_measurement,
        vertical_jump
    INTO v_latest_jump_data
    FROM physical_metrics
    WHERE id_player = p_player_id
    ORDER BY date_of_measurement DESC
    LIMIT 1;
    
    report_result.latest_jump_date := v_latest_jump_data.date_of_measurement;
    report_result.latest_vertical_jump := v_latest_jump_data.vertical_jump;

    -- Calculate weighted total score and build breakdown array
    FOR v_metric_record IN 
        SELECT 
            metric_name, 
            metric_weight, 
            average_value,
            session_count
        FROM get_player_season_metric_averages(p_player_id, p_season_id)
    LOOP
        -- Simple scoring: average value * metric weight
        v_total_score := v_total_score + (v_metric_record.average_value * v_metric_record.metric_weight);
        
        -- Build breakdown detail
        v_metric_detail.metric_name := v_metric_record.metric_name;
        v_metric_detail.metric_weight := v_metric_record.metric_weight;
        v_metric_detail.average_value := v_metric_record.average_value;
        v_metric_breakdown := array_append(v_metric_breakdown, v_metric_detail);

        -- Record the count of sessions analyzed (assuming all metrics come from the same set of sessions)
        report_result.total_sessions_analyzed := COALESCE(v_metric_record.session_count, 0);

    END LOOP;

    report_result.total_scouting_score := ROUND(v_total_score, 3);
    report_result.metric_breakdown := v_metric_breakdown;

    -- Fix: Cap the normalized score to prevent overflow
    IF v_max_possible_score > 0 THEN
        -- Calculate normalized score and cap it at 99.99 to prevent overflow
        report_result.normalized_score := LEAST(
            ROUND((v_total_score / v_max_possible_score) * p_normalization_max_score, 2),
            99.99
        );
    ELSE
        report_result.normalized_score := 0.00;
    END IF;

    RETURN report_result;
END;
$$ LANGUAGE plpgsql;


-- VIEW: Expose the scouting report as a table-like view
CREATE OR REPLACE VIEW player_scouting_report_view AS
SELECT 
    (generate_player_scouting_summary(p.id_player, (SELECT MAX(id_season) FROM season))).*
FROM player p;