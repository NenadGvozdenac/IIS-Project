using MassTransit;
using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Application.Saga.Messages;

namespace match_service.src.Matches.Core.Application.Features.TeamMember.DeleteTeamMember
{
    public class DeleteTeamMemberHandler : IRequestHandler<DeleteTeamMemberCommand, Result<DeleteTeamMemberResponse>>
    {
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public DeleteTeamMemberHandler(ITeamMemberRepository teamMemberRepository, IPublishEndpoint publishEndpoint)
        {
            _teamMemberRepository = teamMemberRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Result<DeleteTeamMemberResponse>> Handle(DeleteTeamMemberCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!_teamMemberRepository.Exists(request.IdPlayer, request.IdTeam))
                {
                    return Result<DeleteTeamMemberResponse>.Failure($"Team member with Player ID {request.IdPlayer} and Team ID {request.IdTeam} not found.")
                        .WithCode((int)ResultCode.NotFound);
                }

                // Start the distributed transaction saga
                var sagaCommand = new DeleteTeamMemberSagaCommand
                {
                    CorrelationId = Guid.NewGuid(),
                    PlayerId = request.IdPlayer,
                    TeamId = request.IdTeam,
                    RequesterId = "system", // Could be from the current user context
                    RequestedAt = DateTime.UtcNow
                };

                await _publishEndpoint.Publish(sagaCommand, cancellationToken);

                var response = new DeleteTeamMemberResponse
                {
                    IsDeleted = true,
                    Message = $"Team member deletion process initiated for Player ID {request.IdPlayer} and Team ID {request.IdTeam}. The deletion will be processed across all services."
                };

                return Result<DeleteTeamMemberResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<DeleteTeamMemberResponse>.Failure($"An error occurred while initiating team member deletion: {ex.Message}")
                    .WithCode((int)ResultCode.InternalServerError);
            }
        }
    }
}
