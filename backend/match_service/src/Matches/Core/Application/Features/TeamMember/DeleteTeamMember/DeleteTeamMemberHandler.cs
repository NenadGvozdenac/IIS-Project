using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Application.Saga.Messages;

namespace match_service.src.Matches.Core.Application.Features.TeamMember.DeleteTeamMember
{
    public class DeleteTeamMemberHandler : IRequestHandler<DeleteTeamMemberCommand, Result<DeleteTeamMemberResponse>>
    {
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<DeleteTeamMemberHandler> _logger;

        public DeleteTeamMemberHandler(ITeamMemberRepository teamMemberRepository, IPublishEndpoint publishEndpoint, ILogger<DeleteTeamMemberHandler> logger)
        {
            _teamMemberRepository = teamMemberRepository;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<Result<DeleteTeamMemberResponse>> Handle(DeleteTeamMemberCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeleteTeamMemberHandler called with Player ID: {PlayerId}, Team ID: {TeamId}", request.IdPlayer, request.IdTeam);
            
            try
            {
                if (!_teamMemberRepository.Exists(request.IdPlayer, request.IdTeam))
                {
                    _logger.LogWarning("Team member not found: Player ID {PlayerId}, Team ID {TeamId}", request.IdPlayer, request.IdTeam);
                    return Result<DeleteTeamMemberResponse>.Failure($"Team member with Player ID {request.IdPlayer} and Team ID {request.IdTeam} not found.")
                        .WithCode((int)ResultCode.NotFound);
                }

                // Start the distributed transaction saga
                var correlationId = Guid.NewGuid();
                var sagaStartedEvent = new DeleteTeamMemberSagaStarted
                {
                    CorrelationId = correlationId,
                    PlayerId = request.IdPlayer,
                    TeamId = request.IdTeam,
                    RequesterId = "system", // Could be from the current user context
                    RequestedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Publishing saga started event with CorrelationId: {CorrelationId}", correlationId);
                await _publishEndpoint.Publish(sagaStartedEvent, cancellationToken);

                var response = new DeleteTeamMemberResponse
                {
                    IsDeleted = false,
                    Message = $"Team member deletion saga initiated with ID: {correlationId}.",
                    CorrelationId = correlationId // Return the ID for status tracking
                };

                _logger.LogInformation("Saga initiated successfully with CorrelationId: {CorrelationId}", correlationId);
                return Result<DeleteTeamMemberResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteTeamMemberHandler for Player ID {PlayerId}, Team ID {TeamId}", request.IdPlayer, request.IdTeam);
                return Result<DeleteTeamMemberResponse>.Failure($"An error occurred while initiating team member deletion: {ex.Message}")
                    .WithCode((int)ResultCode.InternalServerError);
            }
        }
    }
}
