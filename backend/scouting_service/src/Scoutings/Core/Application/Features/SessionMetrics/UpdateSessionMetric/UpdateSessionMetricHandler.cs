using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionMetrics.UpdateSessionMetric;

public class UpdateSessionMetricHandler : IRequestHandler<UpdateSessionMetricCommand, Result<UpdateSessionMetricResponse>>
{
    private readonly ISessionMetricRepository _sessionMetricRepository;

    public UpdateSessionMetricHandler(ISessionMetricRepository sessionMetricRepository)
    {
        _sessionMetricRepository = sessionMetricRepository;
    }

    public Task<Result<UpdateSessionMetricResponse>> Handle(UpdateSessionMetricCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingSessionMetric = _sessionMetricRepository.GetBySessionAndMetric(request.IdSession, request.IdMetrics);
            if (existingSessionMetric == null)
            {
                return Task.FromResult(Result<UpdateSessionMetricResponse>.Failure("SessionMetric not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Update the existing entity's properties instead of creating a new one
            existingSessionMetric.Value = request.Value;

            var updatedSessionMetric = _sessionMetricRepository.Update(existingSessionMetric);

            var response = new UpdateSessionMetricResponse
            {
                Value = updatedSessionMetric.Value,
                IdSession = updatedSessionMetric.IdSession,
                IdMetrics = updatedSessionMetric.IdMetrics
            };

            return Task.FromResult(Result<UpdateSessionMetricResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<UpdateSessionMetricResponse>.Failure($"An error occurred while updating session metric: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}