using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.IsForSeat.DeleteIsForSeat;

public class DeleteIsForSeatRelationshipHandler : IRequestHandler<DeleteIsForSeatRelationshipCommand, Result<DeleteIsForSeatRelationshipResponse>>
{
    private readonly IGraphIsForSeatRelationshipRepository _isForSeatRelationshipRepository;

    public DeleteIsForSeatRelationshipHandler(IGraphIsForSeatRelationshipRepository isForSeatRelationshipRepository)
    {
        _isForSeatRelationshipRepository = isForSeatRelationshipRepository;
    }

    public async Task<Result<DeleteIsForSeatRelationshipResponse>> Handle(DeleteIsForSeatRelationshipCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.IndividualTicketId <= 0)
            {
                return Result<DeleteIsForSeatRelationshipResponse>.Failure("Individual Ticket ID is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            if (request.SeatId <= 0)
            {
                return Result<DeleteIsForSeatRelationshipResponse>.Failure("Seat ID is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            // Check if relationship exists
            var existingRelationship = await _isForSeatRelationshipRepository.GetIsForSeatRelationship(request.IndividualTicketId, request.SeatId);
            if (existingRelationship == null)
            {
                return Result<DeleteIsForSeatRelationshipResponse>.Failure("Is_for_seat relationship not found")
                    .WithCode((int)ResultCode.NotFound);
            }

            await _isForSeatRelationshipRepository.DeleteIsForSeatRelationship(request.IndividualTicketId, request.SeatId);

            var response = new DeleteIsForSeatRelationshipResponse
            {
                IndividualTicketId = request.IndividualTicketId,
                SeatId = request.SeatId,
                Message = "Is_for_seat relationship deleted successfully"
            };

            return Result<DeleteIsForSeatRelationshipResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<DeleteIsForSeatRelationshipResponse>.Failure($"An error occurred while deleting the is_for_seat relationship: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}