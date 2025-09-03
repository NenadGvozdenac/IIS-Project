using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.Teams.GetAllTeams;

public class GetAllTeamsHandler : IRequestHandler<GetAllTeamsQuery, Result<List<GetAllTeamsResponse>>>
{
    private readonly ITeamRepository _teamRepository;

    public GetAllTeamsHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public Task<Result<List<GetAllTeamsResponse>>> Handle(GetAllTeamsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var teams = _teamRepository.GetAll();
            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            var response = teams.Select(t => new GetAllTeamsResponse
            {
                IdTeam = t.IdTeam,
                Name = t.Name,
                City = t.City,
                State = t.State,
                Hall = t.Hall
            }).ToList();

            return Task.FromResult(Result<List<GetAllTeamsResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetAllTeamsResponse>>.Failure($"An error occurred while retrieving teams: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
