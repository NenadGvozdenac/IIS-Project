using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetMatchRevenue;

public class GetMatchRevenueHandler : IRequestHandler<GetMatchRevenueQuery, Result<GetMatchRevenueResponse>>
{
    private readonly IGraphComplexQueryRepository _complexQueryRepository;

    public GetMatchRevenueHandler(IGraphComplexQueryRepository complexQueryRepository)
    {
        _complexQueryRepository = complexQueryRepository;
    }

    public async Task<Result<GetMatchRevenueResponse>> Handle(GetMatchRevenueQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var matches = await _complexQueryRepository.GetMatchesWithRevenueAboveThreshold(request.MinTicketsSold);

            var response = new GetMatchRevenueResponse
            {
                Matches = matches
            };

            return Result<GetMatchRevenueResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetMatchRevenueResponse>.Failure($"An error occurred while getting match revenue: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}