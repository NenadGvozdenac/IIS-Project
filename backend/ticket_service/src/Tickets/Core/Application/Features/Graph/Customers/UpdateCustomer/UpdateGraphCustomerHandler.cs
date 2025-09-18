using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Domain.Entities.Graph;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.UpdateCustomer;

public class UpdateGraphCustomerHandler : IRequestHandler<UpdateGraphCustomerCommand, Result<UpdateGraphCustomerResponse>>
{
    private readonly IGraphCustomerRepository _customerRepository;

    public UpdateGraphCustomerHandler(IGraphCustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<UpdateGraphCustomerResponse>> Handle(UpdateGraphCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = new Customer
            {
                Email = request.NewEmail,
                Name = request.Name,
                Surname = request.Surname,
                Phone = request.Phone,
                Type = request.Type
            };

            await _customerRepository.UpdateCustomer(customer);
            var response = new UpdateGraphCustomerResponse(customer.Email, customer.Name, customer.Surname, customer.Phone, customer.Type);
            return Result<UpdateGraphCustomerResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<UpdateGraphCustomerResponse>.Failure($"Failed to update customer: {ex.Message}");
        }
    }
}