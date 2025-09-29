using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetMatchAveragePrice;

public class GetMatchAveragePriceHandler : IRequestHandler<GetMatchAveragePriceQuery, Result<GetMatchAveragePriceResponse>>
{
    private readonly IGraphComplexQueryRepository _complexQueryRepository;

    public GetMatchAveragePriceHandler(IGraphComplexQueryRepository complexQueryRepository)
    {
        _complexQueryRepository = complexQueryRepository;
    }

    public async Task<Result<GetMatchAveragePriceResponse>> Handle(GetMatchAveragePriceQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var matches = await _complexQueryRepository.GetMatchesWithAveragePrice(request.MinTicketsSold);

            var response = new GetMatchAveragePriceResponse
            {
                Matches = matches
            };

            return Result<GetMatchAveragePriceResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetMatchAveragePriceResponse>.Failure($"An error occurred while getting matches with average price: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}