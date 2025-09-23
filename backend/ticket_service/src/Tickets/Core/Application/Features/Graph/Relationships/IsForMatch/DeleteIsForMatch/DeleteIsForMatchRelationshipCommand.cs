using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.IsForMatch.DeleteIsForMatch;

public class DeleteIsForMatchRelationshipCommand : IRequest<Result<DeleteIsForMatchRelationshipResponse>>
{
    public int IndividualTicketId { get; set; }
    public int MatchId { get; set; }

    public DeleteIsForMatchRelationshipCommand(int individualTicketId, int matchId)
    {
        IndividualTicketId = individualTicketId;
        MatchId = matchId;
    }
}