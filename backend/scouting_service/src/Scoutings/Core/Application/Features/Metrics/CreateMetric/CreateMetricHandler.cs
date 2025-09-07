using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Features.Metrics.CreateMetric;

public class CreateMetricHandler : IRequestHandler<CreateMetricCommand, Result<CreateMetricResponse>>
{
    private readonly IMetricRepository _metricRepository;

    public CreateMetricHandler(IMetricRepository metricRepository)
    {
        _metricRepository = metricRepository;
    }

    public Task<Result<CreateMetricResponse>> Handle(CreateMetricCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var metric = new Metric
            {
                Name = request.Name,
                IsPermanent = request.IsPermanent,
                MetricWeight = request.MetricWeight,
                IdUser = request.IdUser,
                IdMetricType = request.IdMetricType
            };

            var createdMetric = _metricRepository.Create(metric);

            var response = new CreateMetricResponse
            {
                IdMetrics = createdMetric.IdMetrics,
                Name = createdMetric.Name,
                IsPermanent = createdMetric.IsPermanent,
                MetricWeight = createdMetric.MetricWeight,
                IdUser = createdMetric.IdUser,
                IdMetricType = createdMetric.IdMetricType
            };

            return Task.FromResult(Result<CreateMetricResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreateMetricResponse>.Failure($"An error occurred while creating metric: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
