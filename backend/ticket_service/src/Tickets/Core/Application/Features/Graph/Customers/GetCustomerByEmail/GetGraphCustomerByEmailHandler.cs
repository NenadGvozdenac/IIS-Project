using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.GetCustomerByEmail;

public class GetGraphCustomerByEmailHandler : IRequestHandler<GetGraphCustomerByEmailQuery, Result<GetGraphCustomerByEmailResponse>>
{
    private readonly IGraphCustomerRepository _graphCustomerRepository;

    public GetGraphCustomerByEmailHandler(IGraphCustomerRepository graphCustomerRepository)
    {
        _graphCustomerRepository = graphCustomerRepository;
    }

    public async Task<Result<GetGraphCustomerByEmailResponse>> Handle(GetGraphCustomerByEmailQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _graphCustomerRepository.GetCustomerByEmail(request.Email);
            if (customer == null)
            {
                return Result<GetGraphCustomerByEmailResponse>.Failure($"Customer with email '{request.Email}' not found")
                    .WithCode((int)ResultCode.NotFound);
            }

            var response = new GetGraphCustomerByEmailResponse
            {
                Email = customer.Email,
                Name = customer.Name,
                Surname = customer.Surname,
                Phone = customer.Phone,
                Type = customer.Type
            };

            return Result<GetGraphCustomerByEmailResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetGraphCustomerByEmailResponse>.Failure($"An error occurred while retrieving the customer: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}