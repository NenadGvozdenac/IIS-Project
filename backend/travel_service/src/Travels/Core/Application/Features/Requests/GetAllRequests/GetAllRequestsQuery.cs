using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Requests.GetAllRequests;

public record GetAllRequestsQuery(string Type, int IdMatch) : IRequest<Result<GetAllRequestsResponse>>;