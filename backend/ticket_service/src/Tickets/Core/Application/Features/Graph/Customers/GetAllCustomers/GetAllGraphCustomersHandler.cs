using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Customers.GetAllCustomers;

public class GetAllGraphCustomersHandler : IRequestHandler<GetAllGraphCustomersQuery, Result<List<GetAllGraphCustomersResponse>>>
{
    private readonly IGraphCustomerRepository _graphCustomerRepository;

    public GetAllGraphCustomersHandler(IGraphCustomerRepository graphCustomerRepository)
    {
        _graphCustomerRepository = graphCustomerRepository;
    }

    public async Task<Result<List<GetAllGraphCustomersResponse>>> Handle(GetAllGraphCustomersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var customers = await _graphCustomerRepository.GetAllCustomers();
            var response = customers.Select(customer => new GetAllGraphCustomersResponse
            {
                Id = customer.Id,
                ElementId = customer.ElementId,
                Email = customer.Email,
                Name = customer.Name,
                Surname = customer.Surname,
                Phone = customer.Phone,
                Type = customer.Type
            }).ToList();

            return Result<List<GetAllGraphCustomersResponse>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<List<GetAllGraphCustomersResponse>>.Failure($"An error occurred while retrieving customers: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}