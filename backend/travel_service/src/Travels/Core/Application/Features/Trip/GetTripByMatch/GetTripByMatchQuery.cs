using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Trip.GetTripByMatch;

public class GetTripByMatchQuery : IRequest<Result<GetTripByMatchResponse>>
{
    public int MatchId { get; set; }

    public GetTripByMatchQuery(int matchId)
    {
        MatchId = matchId;
    }
}