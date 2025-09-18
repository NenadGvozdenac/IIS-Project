using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;
using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.CreateCustomer;

public class CreateGraphCustomerHandler : IRequestHandler<CreateGraphCustomerCommand, Result<CreateGraphCustomerResponse>>
{
    private readonly IGraphCustomerRepository _graphCustomerRepository;

    public CreateGraphCustomerHandler(IGraphCustomerRepository graphCustomerRepository)
    {
        _graphCustomerRepository = graphCustomerRepository;
    }

    public async Task<Result<CreateGraphCustomerResponse>> Handle(CreateGraphCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Result<CreateGraphCustomerResponse>.Failure("Customer email is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result<CreateGraphCustomerResponse>.Failure("Customer name is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            // Check if customer already exists
            var existingCustomer = await _graphCustomerRepository.GetCustomerByEmail(request.Email);
            if (existingCustomer != null)
            {
                return Result<CreateGraphCustomerResponse>.Failure($"Customer with email '{request.Email}' already exists")
                    .WithCode((int)ResultCode.Conflict);
            }

            var customer = new Customer
            {
                Email = request.Email,
                Name = request.Name,
                Surname = request.Surname,
                Phone = request.Phone,
                Type = request.Type
            };

            customer = await _graphCustomerRepository.CreateCustomer(customer);

            if (customer == null)
            {
                return Result<CreateGraphCustomerResponse>.Failure("Failed to create customer")
                    .WithCode((int)ResultCode.InternalServerError);
            }

            var response = new CreateGraphCustomerResponse
            {
                Id = customer.Id,
                ElementId = customer.ElementId,
                Email = customer.Email,
                Name = customer.Name,
                Surname = customer.Surname,
                Phone = customer.Phone,
                Type = customer.Type
            };

            return Result<CreateGraphCustomerResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<CreateGraphCustomerResponse>.Failure($"An error occurred while creating the customer: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}