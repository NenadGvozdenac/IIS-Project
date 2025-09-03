using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Matches.GetMatchById;

public class GetMatchByIdHandler : IRequestHandler<GetMatchByIdQuery, Result<GetMatchByIdResponse>>
{
    private readonly IMatchRepository _matchRepository;

    public GetMatchByIdHandler(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public Task<Result<GetMatchByIdResponse>> Handle(GetMatchByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var match = _matchRepository.GetById(request.Id);

            if (match == null)
            {
                return Task.FromResult(Result<GetMatchByIdResponse>.Failure($"Match with ID {request.Id} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var response = new GetMatchByIdResponse
            {
                IdMatch = match.IdMatch,
                Name = match.Name,
                ScheduledAt = match.ScheduledAt,
                Type = match.Type,
                State = match.State,
                City = match.City,
                Hall = match.Hall,
                IsInOurHall = match.IsInOurHall,
                TransportationRequired = match.TransportationRequired,
                AccommodationRequired = match.AccommodationRequired,
                CompetitionName = match.IdCompetitionNavigation?.Name,
                SeasonName = match.IdSeasonNavigation.Name,
                TeamName = match.IdTeamNavigation.Name
            };

            return Task.FromResult(Result<GetMatchByIdResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetMatchByIdResponse>.Failure($"An error occurred while retrieving the match: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
