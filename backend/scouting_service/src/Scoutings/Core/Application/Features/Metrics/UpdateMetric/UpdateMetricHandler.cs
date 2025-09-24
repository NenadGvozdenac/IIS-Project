using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Features.Metrics.UpdateMetric;

public class UpdateMetricHandler : IRequestHandler<UpdateMetricCommand, Result<UpdateMetricResponse>>
{
    private readonly IMetricRepository _metricRepository;

    public UpdateMetricHandler(IMetricRepository metricRepository)
    {
        _metricRepository = metricRepository;
    }

    public Task<Result<UpdateMetricResponse>> Handle(UpdateMetricCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingMetric = _metricRepository.GetById(request.IdMetrics);
            if (existingMetric == null)
            {
                return Task.FromResult(Result<UpdateMetricResponse>.Failure("Metric not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Update the existing entity's properties instead of creating a new one
            existingMetric.Name = request.Name;
            existingMetric.IsPermanent = request.IsPermanent;
            existingMetric.MetricWeight = request.MetricWeight;
            existingMetric.IdUser = request.IdUser;
            existingMetric.IdMetricType = request.IdMetricType;

            var updatedMetric = _metricRepository.Update(existingMetric);

            var response = new UpdateMetricResponse
            {
                IdMetrics = updatedMetric.IdMetrics,
                Name = updatedMetric.Name,
                IsPermanent = updatedMetric.IsPermanent,
                MetricWeight = updatedMetric.MetricWeight,
                IdUser = updatedMetric.IdUser,
                IdMetricType = updatedMetric.IdMetricType
            };

            return Task.FromResult(Result<UpdateMetricResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<UpdateMetricResponse>.Failure($"An error occurred while updating metric: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}