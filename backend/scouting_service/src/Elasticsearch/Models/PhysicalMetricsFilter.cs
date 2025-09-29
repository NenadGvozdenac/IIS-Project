namespace scouting_service.src.Elasticsearch.Models
{
    public class PhysicalMetricsFilter
    {
        public float? MinVerticalJump { get; set; }
        public float? MaxFatPercentage { get; set; }
        public float? MinWingspan { get; set; }
        public int Size { get; set; } = 20;
    }
}