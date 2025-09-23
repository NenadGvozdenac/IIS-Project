using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Relationships.IsForMatch.CreateIsForMatch;

public class CreateIsForMatchRelationshipCommand : IRequest<Result<CreateIsForMatchRelationshipResponse>>
{
    public int IndividualTicketId { get; set; }
    public int MatchId { get; set; }

    public CreateIsForMatchRelationshipCommand(int individualTicketId, int matchId)
    {
        IndividualTicketId = individualTicketId;
        MatchId = matchId;
    }
}