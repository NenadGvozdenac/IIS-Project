using Nest;

namespace scouting_service.src.Elasticsearch.Models
{
    [ElasticsearchType(IdProperty = nameof(Id))]
    public class PlayerDocument
    {
        public int Id { get; set; }

        [Text(Analyzer = "standard")]
        [Keyword(Name = "name.keyword")]
        public string Name { get; set; } = string.Empty;

        [Text(Analyzer = "standard")]
        [Keyword(Name = "surname.keyword")]
        public string Surname { get; set; } = string.Empty;

        [Date(Format = "yyyy-MM-dd")]
        public DateTime Birthday { get; set; }

        public float Weight { get; set; }

        public float Height { get; set; }

        [Object]
        public PositionInfo Position { get; set; } = new();

        [Object]
        public NationalityInfo Nationality { get; set; } = new();

        [Nested]
        public List<PhysicalMetric> PhysicalMetrics { get; set; } = new();

        [Text(Analyzer = "standard")]
        public string Bio { get; set; } = string.Empty;

        [Text(Analyzer = "standard")]
        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public class PositionInfo
    {
        public int Id { get; set; }

        [Text(Analyzer = "standard")]
        [Keyword(Name = "name.keyword")]
        public string Name { get; set; } = string.Empty;
    }

    public class NationalityInfo
    {
        public int Id { get; set; }

        [Text(Analyzer = "standard")]
        [Keyword(Name = "state.keyword")]
        public string State { get; set; } = string.Empty;
    }

    public class PhysicalMetric
    {
        public int Id { get; set; }
        public float VerticalJump { get; set; }
        public float FatPercentage { get; set; }
        public float BenchPressWeight { get; set; }
        public float SquatWeight { get; set; }
        public float SprintSpeed { get; set; }
        public float Weight { get; set; }
        public float Height { get; set; }
        public float Wingspan { get; set; }

        [Date(Format = "yyyy-MM-dd")]
        public DateTime DateOfMeasurement { get; set; }
    }
}