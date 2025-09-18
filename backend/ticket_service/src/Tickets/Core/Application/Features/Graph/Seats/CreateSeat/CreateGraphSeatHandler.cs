using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;
using ticket_service.src.Tickets.Core.Domain.Entities.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.CreateSeat;

public class CreateGraphSeatHandler : IRequestHandler<CreateGraphSeatCommand, Result<CreateGraphSeatResponse>>
{
    private readonly IGraphSeatRepository _graphSeatRepository;

    public CreateGraphSeatHandler(IGraphSeatRepository graphSeatRepository)
    {
        _graphSeatRepository = graphSeatRepository;
    }

    public async Task<Result<CreateGraphSeatResponse>> Handle(CreateGraphSeatCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result<CreateGraphSeatResponse>.Failure("Seat name is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(request.Direction))
            {
                return Result<CreateGraphSeatResponse>.Failure("Seat direction is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            // Check if seat already exists
            var existingSeat = await _graphSeatRepository.GetSeatByName(request.Name);
            if (existingSeat != null)
            {
                return Result<CreateGraphSeatResponse>.Failure($"Seat with name '{request.Name}' already exists")
                    .WithCode((int)ResultCode.Conflict);
            }

            var seat = new Seat
            {
                Name = request.Name,
                Row = request.Row,
                Number = request.Number,
                Direction = request.Direction
            };

            await _graphSeatRepository.CreateSeat(seat);

            var response = new CreateGraphSeatResponse
            {
                Name = seat.Name,
                Row = seat.Row,
                Number = seat.Number,
                Direction = seat.Direction
            };

            return Result<CreateGraphSeatResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<CreateGraphSeatResponse>.Failure($"An error occurred while creating the seat: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}