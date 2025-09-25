using MediatR;
using Microsoft.EntityFrameworkCore;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionMetrics.GetAllSessionMetrics;

public class GetAllSessionMetricsHandler : IRequestHandler<GetAllSessionMetricsQuery, Result<GetAllSessionMetricsResponse>>
{
    private readonly ISessionMetricRepository _sessionMetricRepository;

    public GetAllSessionMetricsHandler(ISessionMetricRepository sessionMetricRepository)
    {
        _sessionMetricRepository = sessionMetricRepository;
    }

    public Task<Result<GetAllSessionMetricsResponse>> Handle(GetAllSessionMetricsQuery request, CancellationToken cancellationToken)
    {
        var sessionMetrics = _sessionMetricRepository.GetAll();

        var sessionMetricDtos = sessionMetrics.Select(sm => new SessionMetricDto
        {
            Value = sm.Value,
            IdSession = sm.IdSession,
            IdMetrics = sm.IdMetrics,
            MetricName = sm.IdMetricsNavigation?.Name ?? string.Empty,
            MetricTypeName = sm.IdMetricsNavigation?.IdMetricTypeNavigation?.Type ?? string.Empty
        }).ToList();

        var response = new GetAllSessionMetricsResponse
        {
            SessionMetrics = sessionMetricDtos
        };

        return Task.FromResult(Result<GetAllSessionMetricsResponse>.Success(response));
    }
}
