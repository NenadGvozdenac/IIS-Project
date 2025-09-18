using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Seats.DeleteSeat;

public class DeleteGraphSeatHandler : IRequestHandler<DeleteGraphSeatCommand, Result<DeleteGraphSeatResponse>>
{
    private readonly IGraphSeatRepository _graphSeatRepository;

    public DeleteGraphSeatHandler(IGraphSeatRepository graphSeatRepository)
    {
        _graphSeatRepository = graphSeatRepository;
    }

    public async Task<Result<DeleteGraphSeatResponse>> Handle(DeleteGraphSeatCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if seat exists
            var existingSeat = await _graphSeatRepository.GetSeatByName(request.Name);
            if (existingSeat == null)
            {
                return Result<DeleteGraphSeatResponse>.Failure($"Seat with name '{request.Name}' not found")
                    .WithCode((int)ResultCode.NotFound);
            }

            await _graphSeatRepository.DeleteSeat(request.Name);

            var response = new DeleteGraphSeatResponse
            {
                Message = $"Seat '{request.Name}' deleted successfully"
            };

            return Result<DeleteGraphSeatResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<DeleteGraphSeatResponse>.Failure($"An error occurred while deleting the seat: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}