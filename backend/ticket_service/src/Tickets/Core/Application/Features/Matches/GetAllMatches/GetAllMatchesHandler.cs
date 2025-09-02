using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Matches.GetAllMatches;

public class GetAllMatchesHandler : IRequestHandler<GetAllMatchesQuery, Result<IEnumerable<GetAllMatchesResponse>>>
{
    private readonly IMatchRepository _matchRepository;

    public GetAllMatchesHandler(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public Task<Result<IEnumerable<GetAllMatchesResponse>>> Handle(GetAllMatchesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var matches = _matchRepository.GetAll();

            var response = matches.Select(match => new GetAllMatchesResponse
            {
                IdMatch = match.IdMatch,
                Name = match.Name,
                CreatedAt = match.CreatedAt,
                Type = match.Type,
                State = match.State,
                City = match.City,
                Hall = match.Hall,
                IsInOurHall = match.IsInOurHall == 1,
                TransportationRequired = match.TransportationRequired == 1,
                AccommodationRequired = match.AccommodationRequired == 1,
                CompetitionName = match.IdCompetitionNavigation?.Name,
                SeasonName = match.IdSeasonNavigation.Name,
                TeamName = match.IdTeamNavigation.Name
            });

            return Task.FromResult(Result<IEnumerable<GetAllMatchesResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<IEnumerable<GetAllMatchesResponse>>.Failure($"An error occurred while retrieving matches: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
