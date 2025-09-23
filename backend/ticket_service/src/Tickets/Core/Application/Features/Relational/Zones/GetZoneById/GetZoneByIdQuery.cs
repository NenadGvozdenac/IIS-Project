using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Zones.GetZoneById;

public record GetZoneByIdQuery(int Id) : IRequest<Result<GetZoneByIdResponse>>;
