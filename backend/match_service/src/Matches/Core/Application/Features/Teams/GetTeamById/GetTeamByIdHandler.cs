using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.Teams.GetTeamById;

public class GetTeamByIdHandler : IRequestHandler<GetTeamByIdQuery, Result<GetTeamByIdResponse>>
{
    private readonly ITeamRepository _teamRepository;

    public GetTeamByIdHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public Task<Result<GetTeamByIdResponse>> Handle(GetTeamByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var team = _teamRepository.GetById(request.Id);

            if (team == null)
            {
                return Task.FromResult(Result<GetTeamByIdResponse>.Failure($"Team with ID {request.Id} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var response = new GetTeamByIdResponse
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
            };

            return Task.FromResult(Result<GetTeamByIdResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetTeamByIdResponse>.Failure($"An error occurred while retrieving the team: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
