using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.GetSeatsByDirection;

public class GetGraphSeatsByDirectionHandler : IRequestHandler<GetGraphSeatsByDirectionQuery, Result<List<GetGraphSeatsByDirectionResponse>>>
{
    private readonly IGraphSeatRepository _graphSeatRepository;

    public GetGraphSeatsByDirectionHandler(IGraphSeatRepository graphSeatRepository)
    {
        _graphSeatRepository = graphSeatRepository;
    }

    public async Task<Result<List<GetGraphSeatsByDirectionResponse>>> Handle(GetGraphSeatsByDirectionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var seats = await _graphSeatRepository.GetSeatsByDirection(request.Direction);
            var response = seats.Select(seat => new GetGraphSeatsByDirectionResponse
            {
                Name = seat.Name,
                Row = seat.Row,
                Number = seat.Number,
                Direction = seat.Direction
            }).ToList();

            return Result<List<GetGraphSeatsByDirectionResponse>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<List<GetGraphSeatsByDirectionResponse>>.Failure($"An error occurred while retrieving seats by direction: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}