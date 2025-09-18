using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Domain.Entities.Graph;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.DeleteCustomer;

public class DeleteGraphCustomerHandler : IRequestHandler<DeleteGraphCustomerCommand, Result<DeleteGraphCustomerResponse>>
{
    private readonly IGraphCustomerRepository _customerRepository;

    public DeleteGraphCustomerHandler(IGraphCustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<DeleteGraphCustomerResponse>> Handle(DeleteGraphCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _customerRepository.DeleteCustomer(request.Email);
            var response = new DeleteGraphCustomerResponse(request.Email, "Customer deleted successfully");
            return Result<DeleteGraphCustomerResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<DeleteGraphCustomerResponse>.Failure($"Failed to delete customer: {ex.Message}");
        }
    }
}