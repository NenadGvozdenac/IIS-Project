using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.IsForMatch.CreateIsForMatch;

public class CreateIsForMatchRelationshipHandler : IRequestHandler<CreateIsForMatchRelationshipCommand, Result<CreateIsForMatchRelationshipResponse>>
{
    private readonly IGraphIsForMatchRelationshipRepository _isForMatchRelationshipRepository;

    public CreateIsForMatchRelationshipHandler(IGraphIsForMatchRelationshipRepository isForMatchRelationshipRepository)
    {
        _isForMatchRelationshipRepository = isForMatchRelationshipRepository;
    }

    public async Task<Result<CreateIsForMatchRelationshipResponse>> Handle(CreateIsForMatchRelationshipCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.IndividualTicketId <= 0)
            {
                return Result<CreateIsForMatchRelationshipResponse>.Failure("Individual Ticket ID is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            if (request.MatchId <= 0)
            {
                return Result<CreateIsForMatchRelationshipResponse>.Failure("Match ID is required")
                    .WithCode((int)ResultCode.BadRequest);
            }

            // Check if relationship already exists
            var existingRelationship = await _isForMatchRelationshipRepository.GetIsForMatchRelationship(request.IndividualTicketId, request.MatchId);
            if (existingRelationship != null)
            {
                return Result<CreateIsForMatchRelationshipResponse>.Failure("This ticket is already assigned to this match")
                    .WithCode((int)ResultCode.Conflict);
            }

            var relationship = await _isForMatchRelationshipRepository.CreateIsForMatchRelationship(
                request.IndividualTicketId, 
                request.MatchId);

            if (relationship == null)
            {
                return Result<CreateIsForMatchRelationshipResponse>.Failure("Failed to create is_for_match relationship. This ticket may already be assigned to another match.")
                    .WithCode((int)ResultCode.Conflict);
            }

            var response = new CreateIsForMatchRelationshipResponse
            {
                IndividualTicketId = relationship.IndividualTicketId,
                MatchId = relationship.MatchId,
                RelationshipElementId = relationship.RelationshipElementId
            };

            return Result<CreateIsForMatchRelationshipResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<CreateIsForMatchRelationshipResponse>.Failure($"An error occurred while creating the is_for_match relationship: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}