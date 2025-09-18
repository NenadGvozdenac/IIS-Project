using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;
using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.UpdateSeat;

public class UpdateGraphSeatHandler : IRequestHandler<UpdateGraphSeatCommand, Result<UpdateGraphSeatResponse>>
{
    private readonly IGraphSeatRepository _graphSeatRepository;

    public UpdateGraphSeatHandler(IGraphSeatRepository graphSeatRepository)
    {
        _graphSeatRepository = graphSeatRepository;
    }

    public async Task<Result<UpdateGraphSeatResponse>> Handle(UpdateGraphSeatCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Direction))
            {
                return Result<UpdateGraphSeatResponse>.Failure("Seat direction is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            // Check if seat exists
            var existingSeat = await _graphSeatRepository.GetSeatByName(request.Name);
            if (existingSeat == null)
            {
                return Result<UpdateGraphSeatResponse>.Failure($"Seat with name '{request.Name}' not found")
                    .WithCode((int)ResultCode.NotFound);
            }

            var updatedSeat = new Seat
            {
                Name = string.IsNullOrWhiteSpace(request.NewName) ? request.Name : request.NewName,
                Row = request.Row,
                Number = request.Number,
                Direction = request.Direction
            };

            await _graphSeatRepository.UpdateSeat(updatedSeat);

            var response = new UpdateGraphSeatResponse
            {
                Name = updatedSeat.Name,
                Row = updatedSeat.Row,
                Number = updatedSeat.Number,
                Direction = updatedSeat.Direction
            };

            return Result<UpdateGraphSeatResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<UpdateGraphSeatResponse>.Failure($"An error occurred while updating the seat: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}