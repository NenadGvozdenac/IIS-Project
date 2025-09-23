using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.GetCustomerById;

public class GetGraphCustomerByIdHandler : IRequestHandler<GetGraphCustomerByIdQuery, Result<GetGraphCustomerByIdResponse>>
{
    private readonly IGraphCustomerRepository _customerRepository;

    public GetGraphCustomerByIdHandler(IGraphCustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<GetGraphCustomerByIdResponse>> Handle(GetGraphCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _customerRepository.GetCustomerById(request.Id);
            if (customer == null)
            {
                return Result<GetGraphCustomerByIdResponse>.Failure("Customer not found");
            }

            var response = new GetGraphCustomerByIdResponse(customer.Id, customer.ElementId, customer.Email, customer.Name, customer.Surname, customer.Phone, customer.Type);
            return Result<GetGraphCustomerByIdResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetGraphCustomerByIdResponse>.Failure($"Failed to get customer by id: {ex.Message}");
        }
    }
}