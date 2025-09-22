using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Requests.GetRequestsByMatch;

public record GetRequestsByMatchQuery(int MatchId) : IRequest<Result<GetRequestsByMatchResponse>>;