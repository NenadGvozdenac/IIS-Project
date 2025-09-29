using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.Metrics.DeleteMetric;

public class DeleteMetricHandler : IRequestHandler<DeleteMetricCommand, Result<DeleteMetricResponse>>
{
    private readonly IMetricRepository _metricRepository;

    public DeleteMetricHandler(IMetricRepository metricRepository)
    {
        _metricRepository = metricRepository;
    }

    public Task<Result<DeleteMetricResponse>> Handle(DeleteMetricCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingMetric = _metricRepository.GetById(request.IdMetrics);
            if (existingMetric == null)
            {
                return Task.FromResult(Result<DeleteMetricResponse>.Failure("Metric not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Check if metric is permanent - permanent metrics cannot be deleted
            if (existingMetric.IsPermanent == 1) // 1 = true
            {
                return Task.FromResult(Result<DeleteMetricResponse>.Failure("Cannot delete permanent metrics")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Delete the metric
            _metricRepository.Delete(request.IdMetrics);

            var response = new DeleteMetricResponse
            {
                IdMetrics = request.IdMetrics,
                Message = $"Metric '{existingMetric.Name}' deleted successfully"
            };

            return Task.FromResult(Result<DeleteMetricResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<DeleteMetricResponse>.Failure($"An error occurred while deleting metric: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
