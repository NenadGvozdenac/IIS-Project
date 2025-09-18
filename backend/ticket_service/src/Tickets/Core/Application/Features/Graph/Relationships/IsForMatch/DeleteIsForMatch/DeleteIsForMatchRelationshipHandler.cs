using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.IsForMatch.DeleteIsForMatch;

public class DeleteIsForMatchRelationshipHandler : IRequestHandler<DeleteIsForMatchRelationshipCommand, Result<DeleteIsForMatchRelationshipResponse>>
{
    private readonly IGraphIsForMatchRelationshipRepository _isForMatchRelationshipRepository;

    public DeleteIsForMatchRelationshipHandler(IGraphIsForMatchRelationshipRepository isForMatchRelationshipRepository)
    {
        _isForMatchRelationshipRepository = isForMatchRelationshipRepository;
    }

    public async Task<Result<DeleteIsForMatchRelationshipResponse>> Handle(DeleteIsForMatchRelationshipCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.IndividualTicketId <= 0)
            {
                return Result<DeleteIsForMatchRelationshipResponse>.Failure("Individual Ticket ID is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            if (request.MatchId <= 0)
            {
                return Result<DeleteIsForMatchRelationshipResponse>.Failure("Match ID is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            // Check if relationship exists
            var existingRelationship = await _isForMatchRelationshipRepository.GetIsForMatchRelationship(request.IndividualTicketId, request.MatchId);
            if (existingRelationship == null)
            {
                return Result<DeleteIsForMatchRelationshipResponse>.Failure("Is_for_match relationship not found")
                    .WithCode((int)ResultCode.NotFound);
            }

            await _isForMatchRelationshipRepository.DeleteIsForMatchRelationship(request.IndividualTicketId, request.MatchId);

            var response = new DeleteIsForMatchRelationshipResponse
            {
                IndividualTicketId = request.IndividualTicketId,
                MatchId = request.MatchId,
                Message = "Is_for_match relationship deleted successfully"
            };

            return Result<DeleteIsForMatchRelationshipResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<DeleteIsForMatchRelationshipResponse>.Failure($"An error occurred while deleting the is_for_match relationship: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}