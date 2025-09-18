using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Zones.GetAllZones;

public record GetAllZonesQuery() : IRequest<Result<GetAllZonesResponse>>;
