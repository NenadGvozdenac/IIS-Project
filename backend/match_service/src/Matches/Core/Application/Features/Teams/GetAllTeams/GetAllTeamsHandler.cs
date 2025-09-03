using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.Teams.GetAllTeams;

public class GetAllTeamsHandler : IRequestHandler<GetAllTeamsQuery, Result<GetAllTeamsResponse>>
{
    private readonly ITeamRepository _teamRepository;

    public GetAllTeamsHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public Task<Result<GetAllTeamsResponse>> Handle(GetAllTeamsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var teams = _teamRepository.GetAll();

            var teamDtos = teams.Select(team => new TeamDto
            {
                IdTeam = team.IdTeam,
                Name = team.Name,
                State = team.State,
                City = team.City,
                Hall = team.Hall,
                FoundedDate = team.FoundedDate,
                Coach = team.Coach,
                PlayingStyle = team.PlayingStyle,
                KeyStrengths = team.KeyStrengths,
                KeyWeaknesses = team.KeyWeaknesses
            });

            var response = new GetAllTeamsResponse
            {
                Teams = teamDtos
            };

            return Task.FromResult(Result<GetAllTeamsResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetAllTeamsResponse>.Failure($"An error occurred while retrieving teams: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
