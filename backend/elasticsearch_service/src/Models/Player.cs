using Nest;

namespace elasticsearch_service.src.Models
{
    [ElasticsearchType(IdProperty = nameof(IdPlayer))]
    public class Player
    {
        [Number(Name = "id_player")]
        public int IdPlayer { get; set; }

        [Text(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [Text(Name = "surname")]
        public string Surname { get; set; } = string.Empty;

        [Text(Name = "full_name")]
        public string FullName { get; set; } = string.Empty;

        [Date(Name = "birthday")]
        public DateTime? Birthday { get; set; }

        [Keyword(Name = "nationality")]
        public string Nationality { get; set; } = string.Empty;

        [Keyword(Name = "position")]
        public string Position { get; set; } = string.Empty;

        [Nested(Name = "physical_metrics")]
        public List<PhysicalMetric> PhysicalMetrics { get; set; } = new();
    }

    public class PhysicalMetric
    {
        [Number(Name = "vertical_jump")]
        public int? VerticalJump { get; set; }

        [Number(Name = "fat_percentage")]
        public int? FatPercentage { get; set; }

        [Number(Name = "bench_press_weight")]
        public int? BenchPressWeight { get; set; }

        [Number(Name = "squat_weight")]
        public int? SquatWeight { get; set; }

        [Number(Name = "sprint_speed")]
        public int? SprintSpeed { get; set; }

        [Number(Name = "weight")]
        public int? Weight { get; set; }

        [Number(Name = "height")]
        public int? Height { get; set; }

        [Number(Name = "wingspan")]
        public int? Wingspan { get; set; }

        [Date(Name = "date_of_measurement")]
        public DateTime? DateOfMeasurement { get; set; }
    }
}