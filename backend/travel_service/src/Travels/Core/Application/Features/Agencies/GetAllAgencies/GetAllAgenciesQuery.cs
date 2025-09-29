using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Agencies.GetAllAgencies;

public record GetAllAgenciesQuery(string Type) : IRequest<Result<GetAllAgenciesResponse>>;