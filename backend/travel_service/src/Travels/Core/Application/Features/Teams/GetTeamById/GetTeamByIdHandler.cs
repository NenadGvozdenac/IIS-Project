using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.Teams.GetTeamById;

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
                City = team.City,
                State = team.State,
                Hall = team.Hall
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
