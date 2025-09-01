using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Seats.DeleteSeat;

public class DeleteSeatHandler : IRequestHandler<DeleteSeatCommand, Result<DeleteSeatResponse>>
{
    private readonly ISeatRepository _seatRepository;

    public DeleteSeatHandler(ISeatRepository seatRepository)
    {
        _seatRepository = seatRepository;
    }

    public async Task<Result<DeleteSeatResponse>> Handle(DeleteSeatCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var seat = _seatRepository.GetById(request.IdSeat);
            if (seat == null)
            {
                return Result<DeleteSeatResponse>.Failure($"Seat with ID {request.IdSeat} not found")
                    .WithCode((int)ResultCode.NotFound);
            }

            var deleted = _seatRepository.Delete(request.IdSeat);
            if (!deleted)
            {
                return Result<DeleteSeatResponse>.Failure($"Failed to delete seat with ID {request.IdSeat}")
                    .WithCode((int)ResultCode.InternalServerError);
            }

            var response = new DeleteSeatResponse
            {
                Success = true,
                Message = $"Seat with ID {request.IdSeat} was successfully deleted"
            };

            return Result<DeleteSeatResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<DeleteSeatResponse>.Failure($"An error occurred while deleting the seat: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}
