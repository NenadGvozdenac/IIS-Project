using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.GetSeatById;

public class GetGraphSeatByIdHandler : IRequestHandler<GetGraphSeatByIdQuery, Result<GetGraphSeatByIdResponse>>
{
    private readonly IGraphSeatRepository _seatRepository;

    public GetGraphSeatByIdHandler(IGraphSeatRepository seatRepository)
    {
        _seatRepository = seatRepository;
    }

    public async Task<Result<GetGraphSeatByIdResponse>> Handle(GetGraphSeatByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var seat = await _seatRepository.GetSeatById(request.Id);
            if (seat == null)
            {
                return Result<GetGraphSeatByIdResponse>.Failure("Seat not found");
            }

            var response = new GetGraphSeatByIdResponse(seat.Id, seat.ElementId, seat.Name, seat.Row, seat.Number, seat.Direction);
            return Result<GetGraphSeatByIdResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetGraphSeatByIdResponse>.Failure($"Failed to get seat by id: {ex.Message}");
        }
    }
}