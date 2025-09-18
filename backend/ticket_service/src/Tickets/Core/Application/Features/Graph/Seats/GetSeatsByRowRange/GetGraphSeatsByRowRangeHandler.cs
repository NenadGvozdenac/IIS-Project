using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.GetSeatsByRowRange;

public class GetGraphSeatsByRowRangeHandler : IRequestHandler<GetGraphSeatsByRowRangeQuery, Result<List<GetGraphSeatsByRowRangeResponse>>>
{
    private readonly IGraphSeatRepository _graphSeatRepository;

    public GetGraphSeatsByRowRangeHandler(IGraphSeatRepository graphSeatRepository)
    {
        _graphSeatRepository = graphSeatRepository;
    }

    public async Task<Result<List<GetGraphSeatsByRowRangeResponse>>> Handle(GetGraphSeatsByRowRangeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.MinRow <= 0 || request.MaxRow <= 0 || request.MinRow > request.MaxRow)
            {
                return Result<List<GetGraphSeatsByRowRangeResponse>>.Failure("Invalid row range provided")
                    .WithCode((int)ResultCode.BadRequest);
            }

            var seats = await _graphSeatRepository.GetSeatsByRowRange(request.MinRow, request.MaxRow);
            var response = seats.Select(seat => new GetGraphSeatsByRowRangeResponse
            {
                Name = seat.Name,
                Row = seat.Row,
                Number = seat.Number,
                Direction = seat.Direction
            }).ToList();

            return Result<List<GetGraphSeatsByRowRangeResponse>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<List<GetGraphSeatsByRowRangeResponse>>.Failure($"An error occurred while retrieving seats by row range: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}