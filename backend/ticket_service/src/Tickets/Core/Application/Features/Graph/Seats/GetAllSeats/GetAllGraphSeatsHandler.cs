using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.GetAllSeats;

public class GetAllGraphSeatsHandler : IRequestHandler<GetAllGraphSeatsQuery, Result<List<GetAllGraphSeatsResponse>>>
{
    private readonly IGraphSeatRepository _graphSeatRepository;

    public GetAllGraphSeatsHandler(IGraphSeatRepository graphSeatRepository)
    {
        _graphSeatRepository = graphSeatRepository;
    }

    public async Task<Result<List<GetAllGraphSeatsResponse>>> Handle(GetAllGraphSeatsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var seats = await _graphSeatRepository.GetAllSeats();
            var response = seats.Select(seat => new GetAllGraphSeatsResponse
            {
                Id = seat.Id,
                ElementId = seat.ElementId,
                Name = seat.Name,
                Row = seat.Row,
                Number = seat.Number,
                Direction = seat.Direction
            }).ToList();

            return Result<List<GetAllGraphSeatsResponse>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<List<GetAllGraphSeatsResponse>>.Failure($"An error occurred while retrieving seats: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}