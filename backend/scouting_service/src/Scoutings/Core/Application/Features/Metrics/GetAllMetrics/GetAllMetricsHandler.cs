using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.Metrics.GetAllMetrics;

public class GetAllMetricsHandler : IRequestHandler<GetAllMetricsQuery, Result<List<GetAllMetricsResponse>>>
{
    private readonly IMetricRepository _metricRepository;

    public GetAllMetricsHandler(IMetricRepository metricRepository)
    {
        _metricRepository = metricRepository;
    }

    public Task<Result<List<GetAllMetricsResponse>>> Handle(GetAllMetricsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var metrics = _metricRepository.GetAll();

            var response = metrics.Select(m => new GetAllMetricsResponse
            {
                IdMetrics = m.IdMetrics,
                Name = m.Name,
                IsPermanent = m.IsPermanent,
                MetricWeight = m.MetricWeight,
                IdUser = m.IdUser,
                UserName = m.IdUserNavigation?.Name,
                IdMetricType = m.IdMetricType,
                MetricTypeName = m.IdMetricTypeNavigation?.Type ?? string.Empty
            }).ToList();

            return Task.FromResult(Result<List<GetAllMetricsResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetAllMetricsResponse>>.Failure($"An error occurred while retrieving metrics: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
