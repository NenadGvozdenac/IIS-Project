using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.SessionMetrics.CreateSessionMetric;

public class CreateSessionMetricHandler : IRequestHandler<CreateSessionMetricCommand, Result<CreateSessionMetricResponse>>
{
    private readonly ISessionMetricRepository _sessionMetricRepository;

    public CreateSessionMetricHandler(ISessionMetricRepository sessionMetricRepository)
    {
        _sessionMetricRepository = sessionMetricRepository;
    }

    public Task<Result<CreateSessionMetricResponse>> Handle(CreateSessionMetricCommand request, CancellationToken cancellationToken)
    {
        // Check if a session metric already exists for this session and metric combination
        var existingSessionMetric = _sessionMetricRepository.GetBySessionAndMetric(request.IdSession, request.IdMetrics);
        
        if (existingSessionMetric != null)
        {
            // Update the existing session metric with new value
            existingSessionMetric.Value = request.Value;
            var updatedSessionMetric = _sessionMetricRepository.Update(existingSessionMetric);
            
            var updateResponse = new CreateSessionMetricResponse
            {
                Value = updatedSessionMetric.Value,
                IdSession = updatedSessionMetric.IdSession,
                IdMetrics = updatedSessionMetric.IdMetrics
            };

            return Task.FromResult(Result<CreateSessionMetricResponse>.Success(updateResponse));
        }

        // Create new session metric if it doesn't exist
        var sessionMetric = new SessionMetric
        {
            Value = request.Value,
            IdSession = request.IdSession,
            IdMetrics = request.IdMetrics
        };

        var createdSessionMetric = _sessionMetricRepository.Create(sessionMetric);

        if (createdSessionMetric == null)
        {
            return Task.FromResult(Result<CreateSessionMetricResponse>.Failure("Failed to create session metric"));
        }

        var response = new CreateSessionMetricResponse
        {
            Value = createdSessionMetric.Value,
            IdSession = createdSessionMetric.IdSession,
            IdMetrics = createdSessionMetric.IdMetrics
        };

        return Task.FromResult(Result<CreateSessionMetricResponse>.Success(response));
    }
}
