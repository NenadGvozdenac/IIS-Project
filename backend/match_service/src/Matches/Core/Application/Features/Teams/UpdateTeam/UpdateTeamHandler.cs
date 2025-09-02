using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Features.Teams.UpdateTeam;

public class UpdateTeamHandler : IRequestHandler<UpdateTeamCommand, Result<UpdateTeamResponse>>
{
    private readonly ITeamRepository _teamRepository;

    public UpdateTeamHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public Task<Result<UpdateTeamResponse>> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validacija obaveznih polja
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Task.FromResult(Result<UpdateTeamResponse>.Failure("Team name is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (string.IsNullOrWhiteSpace(request.State))
            {
                return Task.FromResult(Result<UpdateTeamResponse>.Failure("State is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (string.IsNullOrWhiteSpace(request.City))
            {
                return Task.FromResult(Result<UpdateTeamResponse>.Failure("City is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (string.IsNullOrWhiteSpace(request.Hall))
            {
                return Task.FromResult(Result<UpdateTeamResponse>.Failure("Hall is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Proveri da li tim postoji
            if (!_teamRepository.Exists(request.IdTeam))
            {
                return Task.FromResult(Result<UpdateTeamResponse>.Failure($"Team with ID {request.IdTeam} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var team = new Team
            {
                IdTeam = request.IdTeam,
                Name = request.Name,
                State = request.State,
                City = request.City,
                Hall = request.Hall,
                FoundedDate = request.FoundedDate,
                Coach = request.Coach,
                KeyStrenghts = request.KeyStrenghts,
                KeyWeaknesses = request.KeyWeaknesses
            };

            var updatedTeam = _teamRepository.Update(team);

            if (updatedTeam == null)
            {
                return Task.FromResult(Result<UpdateTeamResponse>.Failure($"Failed to update team with ID {request.IdTeam}")
                    .WithCode((int)ResultCode.InternalServerError));
            }

            var response = new UpdateTeamResponse
            {
                IdTeam = updatedTeam.IdTeam,
                Name = updatedTeam.Name,
                State = updatedTeam.State,
                City = updatedTeam.City,
                Hall = updatedTeam.Hall,
                FoundedDate = updatedTeam.FoundedDate,
                Coach = updatedTeam.Coach,
                KeyStrenghts = updatedTeam.KeyStrenghts,
                KeyWeaknesses = updatedTeam.KeyWeaknesses
            };

            return Task.FromResult(Result<UpdateTeamResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<UpdateTeamResponse>.Failure($"An error occurred while updating the team: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
