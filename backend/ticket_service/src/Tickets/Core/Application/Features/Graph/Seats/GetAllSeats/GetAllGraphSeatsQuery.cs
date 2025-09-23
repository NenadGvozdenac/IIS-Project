using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.GetAllSeats;

public class GetAllGraphSeatsQuery : IRequest<Result<List<GetAllGraphSeatsResponse>>>
{
}