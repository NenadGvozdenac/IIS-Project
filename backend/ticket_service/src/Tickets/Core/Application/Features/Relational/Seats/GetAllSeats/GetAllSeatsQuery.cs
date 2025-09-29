using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Seats.GetAllSeats;

public class GetAllSeatsQuery : IRequest<Result<List<GetAllSeatsResponse>>>
{
}
