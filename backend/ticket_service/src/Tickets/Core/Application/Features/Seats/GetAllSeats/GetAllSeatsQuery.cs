using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Seats.GetAllSeats;

public class GetAllSeatsQuery : IRequest<Result<List<GetAllSeatsResponse>>>
{
}
