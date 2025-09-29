namespace scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerRecommendations;

public class GetPlayerRecommendationsResponse
{
    public List<PlayerRecommendation> Players { get; set; } = new();
    public List<AvailableMetric> AvailableMetrics { get; set; } = new();
    public List<SelectedMetric> SelectedMetrics { get; set; } = new();
}

public class PlayerRecommendation
{
    public int PlayerId { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public decimal Score { get; set; }
    public decimal ScorePercentage { get; set; }
    public List<PlayerMetricValue> MetricValues { get; set; } = new();
}

public class PlayerMetricValue
{
    public int MetricId { get; set; }
    public string MetricName { get; set; } = null!;
    public decimal AverageValue { get; set; }
    public decimal WeightedValue { get; set; }
    public decimal Weight { get; set; }
}

public class AvailableMetric
{
    public int MetricId { get; set; }
    public string MetricName { get; set; } = null!;
    public decimal Weight { get; set; } = 1.0m;
}

public class SelectedMetric
{
    public int MetricId { get; set; }
    public string MetricName { get; set; } = null!;
    public decimal Weight { get; set; }
}