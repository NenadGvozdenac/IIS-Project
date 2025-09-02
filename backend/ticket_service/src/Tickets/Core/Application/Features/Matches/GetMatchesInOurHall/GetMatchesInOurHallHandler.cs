using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Matches.GetMatchesInOurHall;

public class GetMatchesInOurHallHandler : IRequestHandler<GetMatchesInOurHallQuery, Result<IEnumerable<GetMatchesInOurHallResponse>>>
{
    private readonly IMatchRepository _matchRepository;

    public GetMatchesInOurHallHandler(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public Task<Result<IEnumerable<GetMatchesInOurHallResponse>>> Handle(GetMatchesInOurHallQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var matches = _matchRepository.GetMatchesInOurHall();

            var response = matches.Select(match => new GetMatchesInOurHallResponse
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

            return Task.FromResult(Result<IEnumerable<GetMatchesInOurHallResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<IEnumerable<GetMatchesInOurHallResponse>>.Failure($"An error occurred while retrieving matches in our hall: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
