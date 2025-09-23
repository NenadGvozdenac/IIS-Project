using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.ComplexQueries.GetSectorSales;

public class GetSectorSalesHandler : IRequestHandler<GetSectorSalesQuery, Result<GetSectorSalesResponse>>
{
    private readonly IGraphComplexQueryRepository _complexQueryRepository;
    private readonly IGraphMatchRepository _matchRepository;

    public GetSectorSalesHandler(IGraphComplexQueryRepository complexQueryRepository, IGraphMatchRepository matchRepository)
    {
        _complexQueryRepository = complexQueryRepository;
        _matchRepository = matchRepository;
    }

    public async Task<Result<GetSectorSalesResponse>> Handle(GetSectorSalesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.MatchId < 0)
            {
                return Result<GetSectorSalesResponse>.Failure("Match ID is required and must be greater than 0")
                    .WithCode((int)ResultCode.BadRequest);
            }

            if (request.MinTicketsSold < 1)
            {
                return Result<GetSectorSalesResponse>.Failure("Minimum tickets sold must be at least 1")
                    .WithCode((int)ResultCode.BadRequest);
            }

            var match = await _matchRepository.GetMatchById(request.MatchId);

            if (match == null)
            {
                return Result<GetSectorSalesResponse>.Failure($"Match with ID '{request.MatchId}' not found")
                    .WithCode((int)ResultCode.NotFound);
            }

            var result = await _complexQueryRepository.GetSectorSalesForMatch(request.MatchId, request.MinTicketsSold);

            var response = new GetSectorSalesResponse
            {
                MatchId = request.MatchId,
                MatchName = result.MatchName,
                Sectors = result.Sectors
            };

            return Result<GetSectorSalesResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetSectorSalesResponse>.Failure($"An error occurred while getting sector sales: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}