using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.GetAllCustomers;

public class GetAllGraphCustomersQuery : IRequest<Result<List<GetAllGraphCustomersResponse>>>
{
}