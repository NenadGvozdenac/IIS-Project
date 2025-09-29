namespace elasticsearch_service.src.Models.DTOs
{
    public class PlayerSessionPointsDto
    {
        public int IdPlayer { get; set; }
        public string PlayerFullName { get; set; } = string.Empty;
        public int IdSession { get; set; }
        public int Points { get; set; }
    }

    public class PlayerMaxPointsDto
    {
        public int IdPlayer { get; set; }
        public string PlayerFullName { get; set; } = string.Empty;
        public int MaxPoints { get; set; }
        public DateTime SessionDate { get; set; }
    }

    public class PlayerPlayoffMinutesDto
    {
        public int IdPlayer { get; set; }
        public string PlayerFullName { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public int TotalPlayoffMinutes { get; set; }
    }

    public class PlayerMetricSessionDto
    {
        public int IdPlayer { get; set; }
        public string PlayerFullName { get; set; } = string.Empty;
        public int IdSession { get; set; }
        public DateTime SessionDate { get; set; }
        public string MetricName { get; set; } = string.Empty;
        public double MetricValue { get; set; }
    }

    public class MetricTypeDto
    {
        public string MetricName { get; set; } = string.Empty;
        public string MetricType { get; set; } = string.Empty; // "Quantitative" ili "Qualitative"
    }
}