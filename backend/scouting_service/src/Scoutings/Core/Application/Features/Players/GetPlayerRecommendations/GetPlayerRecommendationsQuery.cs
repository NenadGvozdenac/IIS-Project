using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerRecommendations;

public class GetPlayerRecommendationsQuery : IRequest<Result<GetPlayerRecommendationsResponse>>
{
    public List<MetricWeight> MetricWeights { get; set; } = new();

    public GetPlayerRecommendationsQuery(List<MetricWeight> metricWeights)
    {
        MetricWeights = metricWeights;
    }
}

public class MetricWeight
{
    public int MetricId { get; set; }
    public decimal Weight { get; set; }
}