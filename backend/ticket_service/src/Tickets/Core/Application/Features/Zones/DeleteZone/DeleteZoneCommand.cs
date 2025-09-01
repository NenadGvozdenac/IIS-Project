using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Zones.DeleteZone;

public record DeleteZoneCommand(int Id) : IRequest<Result<DeleteZoneResponse>>;
