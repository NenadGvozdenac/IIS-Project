using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerRecommendations;

public class GetPlayerRecommendationsHandler : IRequestHandler<GetPlayerRecommendationsQuery, Result<GetPlayerRecommendationsResponse>>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IMetricRepository _metricRepository;

    public GetPlayerRecommendationsHandler(IPlayerRepository playerRepository, IMetricRepository metricRepository)
    {
        _playerRepository = playerRepository;
        _metricRepository = metricRepository;
    }

    public async Task<Result<GetPlayerRecommendationsResponse>> Handle(GetPlayerRecommendationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get all metrics to separate into selected and available
            var allMetrics = _metricRepository.GetPermanent().ToList();
            var selectedMetricIds = request.MetricWeights.Select(mw => mw.MetricId).ToHashSet();
            
            var selectedMetrics = allMetrics
                .Where(m => selectedMetricIds.Contains(m.IdMetrics))
                .Select(m =>
                {
                    var weight = request.MetricWeights.First(mw => mw.MetricId == m.IdMetrics).Weight;
                    return new SelectedMetric
                    {
                        MetricId = m.IdMetrics,
                        MetricName = m.Name,
                        Weight = weight
                    };
                }).ToList();

            var availableMetrics = allMetrics
                .Where(m => !selectedMetricIds.Contains(m.IdMetrics))
                .Select(m => new AvailableMetric
                {
                    MetricId = m.IdMetrics,
                    MetricName = m.Name,
                    Weight = 1.0m
                }).ToList();

            // Get player recommendations
            var players = await _playerRepository.GetPlayerRecommendationsAsync(request.MetricWeights);

            var response = new GetPlayerRecommendationsResponse
            {
                Players = players,
                SelectedMetrics = selectedMetrics,
                AvailableMetrics = availableMetrics
            };

            return Result<GetPlayerRecommendationsResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetPlayerRecommendationsResponse>
                .Failure($"An error occurred while retrieving player recommendations: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}