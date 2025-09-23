using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.GetSeatByName;

public class GetGraphSeatByNameHandler : IRequestHandler<GetGraphSeatByNameQuery, Result<GetGraphSeatByNameResponse>>
{
    private readonly IGraphSeatRepository _graphSeatRepository;

    public GetGraphSeatByNameHandler(IGraphSeatRepository graphSeatRepository)
    {
        _graphSeatRepository = graphSeatRepository;
    }

    public async Task<Result<GetGraphSeatByNameResponse>> Handle(GetGraphSeatByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var seat = await _graphSeatRepository.GetSeatByName(request.Name);
            if (seat == null)
            {
                return Result<GetGraphSeatByNameResponse>.Failure($"Seat with name '{request.Name}' not found")
                    .WithCode((int)ResultCode.NotFound);
            }

            var response = new GetGraphSeatByNameResponse(seat.Id, seat.ElementId, seat.Name, seat.Row, seat.Number, seat.Direction);

            return Result<GetGraphSeatByNameResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetGraphSeatByNameResponse>.Failure($"An error occurred while retrieving the seat: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}