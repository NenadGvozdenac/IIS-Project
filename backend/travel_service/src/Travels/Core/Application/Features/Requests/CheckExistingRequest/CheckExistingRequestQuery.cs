using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Requests.CheckExistingRequest;

public record CheckExistingRequestQuery(int MatchId, string Type) : IRequest<Result<CheckExistingRequestResponse>>;