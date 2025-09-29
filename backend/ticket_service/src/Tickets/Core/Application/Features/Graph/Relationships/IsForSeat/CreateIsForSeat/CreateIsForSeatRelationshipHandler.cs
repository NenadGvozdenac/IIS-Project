using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.IsForSeat.CreateIsForSeat;

public class CreateIsForSeatRelationshipHandler : IRequestHandler<CreateIsForSeatRelationshipCommand, Result<CreateIsForSeatRelationshipResponse>>
{
    private readonly IGraphIsForSeatRelationshipRepository _isForSeatRelationshipRepository;

    public CreateIsForSeatRelationshipHandler(IGraphIsForSeatRelationshipRepository isForSeatRelationshipRepository)
    {
        _isForSeatRelationshipRepository = isForSeatRelationshipRepository;
    }

    public async Task<Result<CreateIsForSeatRelationshipResponse>> Handle(CreateIsForSeatRelationshipCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.IndividualTicketId <= 0)
            {
                return Result<CreateIsForSeatRelationshipResponse>.Failure("Individual Ticket ID is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            if (request.SeatId <= 0)
            {
                return Result<CreateIsForSeatRelationshipResponse>.Failure("Seat ID is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            // Check if relationship already exists
            var existingRelationship = await _isForSeatRelationshipRepository.GetIsForSeatRelationship(request.IndividualTicketId, request.SeatId);
            if (existingRelationship != null)
            {
                return Result<CreateIsForSeatRelationshipResponse>.Failure("This ticket is already assigned to this seat")
                    .WithCode((int)ResultCode.Conflict);
            }

            var relationship = await _isForSeatRelationshipRepository.CreateIsForSeatRelationship(
                request.IndividualTicketId, 
                request.SeatId);

            if (relationship == null)
            {
                return Result<CreateIsForSeatRelationshipResponse>.Failure("Failed to create is_for_seat relationship. This ticket may already be assigned to another seat.")
                    .WithCode((int)ResultCode.Conflict);
            }

            var response = new CreateIsForSeatRelationshipResponse
            {
                IndividualTicketId = relationship.IndividualTicketId,
                SeatId = relationship.SeatId,
                RelationshipElementId = relationship.RelationshipElementId
            };

            return Result<CreateIsForSeatRelationshipResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<CreateIsForSeatRelationshipResponse>.Failure($"An error occurred while creating the is_for_seat relationship: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}