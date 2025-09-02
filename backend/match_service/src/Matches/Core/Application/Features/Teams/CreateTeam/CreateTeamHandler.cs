using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Features.Teams.CreateTeam;

public class CreateTeamHandler : IRequestHandler<CreateTeamCommand, Result<CreateTeamResponse>>
{
    private readonly ITeamRepository _teamRepository;

    public CreateTeamHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public Task<Result<CreateTeamResponse>> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validacija obaveznih polja
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Task.FromResult(Result<CreateTeamResponse>.Failure("Team name is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (string.IsNullOrWhiteSpace(request.State))
            {
                return Task.FromResult(Result<CreateTeamResponse>.Failure("State is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (string.IsNullOrWhiteSpace(request.City))
            {
                return Task.FromResult(Result<CreateTeamResponse>.Failure("City is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (string.IsNullOrWhiteSpace(request.Hall))
            {
                return Task.FromResult(Result<CreateTeamResponse>.Failure("Hall is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            var team = new Team
            {
                Name = request.Name,
                State = request.State,
                City = request.City,
                Hall = request.Hall,
                FoundedDate = request.FoundedDate,
                Coach = request.Coach,
                KeyStrenghts = request.KeyStrenghts,
                KeyWeaknesses = request.KeyWeaknesses
            };

            var createdTeam = _teamRepository.Create(team);

            var response = new CreateTeamResponse
            {
                IdTeam = createdTeam.IdTeam,
                Name = createdTeam.Name,
                State = createdTeam.State,
                City = createdTeam.City,
                Hall = createdTeam.Hall,
                FoundedDate = createdTeam.FoundedDate,
                Coach = createdTeam.Coach,
                KeyStrenghts = createdTeam.KeyStrenghts,
                KeyWeaknesses = createdTeam.KeyWeaknesses
            };

            return Task.FromResult(Result<CreateTeamResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreateTeamResponse>.Failure($"An error occurred while creating the team: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
