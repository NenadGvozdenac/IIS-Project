using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetCustomerMatches;

public class GetCustomerMatchesHandler : IRequestHandler<GetCustomerMatchesQuery, Result<GetCustomerMatchesResponse>>
{
    private readonly IGraphComplexQueryRepository _complexQueryRepository;

    public GetCustomerMatchesHandler(IGraphComplexQueryRepository complexQueryRepository)
    {
        _complexQueryRepository = complexQueryRepository;
    }

    public async Task<Result<GetCustomerMatchesResponse>> Handle(GetCustomerMatchesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var customers = await _complexQueryRepository.GetCustomersWithMultipleMatches();

            var response = new GetCustomerMatchesResponse
            {
                Customers = customers
            };

            return Result<GetCustomerMatchesResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetCustomerMatchesResponse>.Failure($"An error occurred while getting customers with multiple matches: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}