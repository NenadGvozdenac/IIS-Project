using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Features.TeamMember.CreateTeamMember
{
    public class CreateTeamMemberHandler : IRequestHandler<CreateTeamMemberCommand, Result<CreateTeamMemberResponse>>
    {
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ITeamRepository _teamRepository;

        public CreateTeamMemberHandler(
            ITeamMemberRepository teamMemberRepository,
            IPlayerRepository playerRepository,
            ITeamRepository teamRepository)
        {
            _teamMemberRepository = teamMemberRepository;
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
        }

        public Task<Result<CreateTeamMemberResponse>> Handle(CreateTeamMemberCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if the team member already exists
                if (_teamMemberRepository.Exists(request.IdPlayer, request.IdTeam))
                {
                    return Task.FromResult(Result<CreateTeamMemberResponse>.Failure($"Team member with Player ID {request.IdPlayer} and Team ID {request.IdTeam} already exists.")
                        .WithCode((int)ResultCode.Conflict));
                }

                // Validate that player exists
                if (!_playerRepository.Exists(request.IdPlayer))
                {
                    return Task.FromResult(Result<CreateTeamMemberResponse>.Failure($"Player with ID {request.IdPlayer} not found.")
                        .WithCode((int)ResultCode.NotFound));
                }

                // Validate that team exists
                if (!_teamRepository.Exists(request.IdTeam))
                {
                    return Task.FromResult(Result<CreateTeamMemberResponse>.Failure($"Team with ID {request.IdTeam} not found.")
                        .WithCode((int)ResultCode.NotFound));
                }

                var teamMember = new Domain.Entities.TeamMember
                {
                    JerseyNumber = request.JerseyNumber,
                    Status = request.Status,
                    IdPlayer = request.IdPlayer,
                    IdTeam = request.IdTeam
                };

                _teamMemberRepository.Create(teamMember);

                // Get the created team member with navigation properties
                var createdTeamMember = _teamMemberRepository.GetById(request.IdPlayer, request.IdTeam);

                var response = new CreateTeamMemberResponse
                {
                    TeamMember = new TeamMemberResponse
                    {
                        JerseyNumber = createdTeamMember.JerseyNumber ?? 0,
                        Status = createdTeamMember.Status ?? string.Empty,
                        IdPlayer = createdTeamMember.IdPlayer,
                        IdTeam = createdTeamMember.IdTeam,
                        PlayerName = createdTeamMember.IdPlayerNavigation?.Name ?? string.Empty,
                        PlayerSurname = createdTeamMember.IdPlayerNavigation?.Surname ?? string.Empty,
                        TeamName = createdTeamMember.IdTeamNavigation?.Name ?? string.Empty
                    }
                };

                return Task.FromResult(Result<CreateTeamMemberResponse>.Success(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<CreateTeamMemberResponse>.Failure($"An error occurred while creating team member: {ex.Message}")
                    .WithCode((int)ResultCode.InternalServerError));
            }
        }
    }
}
