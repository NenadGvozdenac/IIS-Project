using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Competitions.GetCompetitionById;

public class GetCompetitionByIdQuery : IRequest<Result<GetCompetitionByIdResponse>>
{
    public int IdCompetition { get; set; }

    public GetCompetitionByIdQuery(int idCompetition)
    {
        IdCompetition = idCompetition;
    }
}
