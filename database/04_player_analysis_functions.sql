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