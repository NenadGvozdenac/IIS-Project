using Nest;

namespace elasticsearch_service.src.Models
{
    [ElasticsearchType(IdProperty = nameof(IdSession))]
    public class Session
    {
        [Number(Name = "id_session")]
        public int IdSession { get; set; }

        [Date(Name = "start_time")]
        public DateTime? StartTime { get; set; }

        [Date(Name = "end_time")]
        public DateTime? EndTime { get; set; }

        [Keyword(Name = "session_status")]
        public string SessionStatus { get; set; } = string.Empty;

        [Keyword(Name = "session_type")]
        public string SessionType { get; set; } = string.Empty;

        [Number(Name = "id_player")]
        public int IdPlayer { get; set; }

        [Text(Name = "player_full_name")]
        public string PlayerFullName { get; set; } = string.Empty;

        [Nested(Name = "metrics")]
        public List<SessionMetric> Metrics { get; set; } = new();
    }

    public class SessionMetric
    {
        [Text(Name = "metric_name")]
        public string MetricName { get; set; } = string.Empty;

        [Keyword(Name = "metric_value")]
        public string MetricValue { get; set; } = string.Empty;
    }
}