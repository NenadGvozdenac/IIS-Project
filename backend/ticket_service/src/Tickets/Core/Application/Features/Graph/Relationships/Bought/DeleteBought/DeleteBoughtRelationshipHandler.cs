using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.Bought.DeleteBought;

public class DeleteBoughtRelationshipHandler : IRequestHandler<DeleteBoughtRelationshipCommand, Result<DeleteBoughtRelationshipResponse>>
{
    private readonly IGraphBoughtRelationshipRepository _boughtRelationshipRepository;

    public DeleteBoughtRelationshipHandler(IGraphBoughtRelationshipRepository boughtRelationshipRepository)
    {
        _boughtRelationshipRepository = boughtRelationshipRepository;
    }

    public async Task<Result<DeleteBoughtRelationshipResponse>> Handle(DeleteBoughtRelationshipCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.CustomerId <= 0)
            {
                return Result<DeleteBoughtRelationshipResponse>.Failure("Customer ID is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            if (request.IndividualTicketId <= 0)
            {
                return Result<DeleteBoughtRelationshipResponse>.Failure("Individual Ticket ID is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            // Check if relationship exists
            var existingRelationship = await _boughtRelationshipRepository.GetBoughtRelationship(request.CustomerId, request.IndividualTicketId);
            if (existingRelationship == null)
            {
                return Result<DeleteBoughtRelationshipResponse>.Failure("Bought relationship not found")
                    .WithCode((int)ResultCode.NotFound);
            }

            await _boughtRelationshipRepository.DeleteBoughtRelationship(request.CustomerId, request.IndividualTicketId);

            var response = new DeleteBoughtRelationshipResponse
            {
                CustomerId = request.CustomerId,
                IndividualTicketId = request.IndividualTicketId,
                Message = "Bought relationship deleted successfully"
            };

            return Result<DeleteBoughtRelationshipResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<DeleteBoughtRelationshipResponse>.Failure($"An error occurred while deleting the bought relationship: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}