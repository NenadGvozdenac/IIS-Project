using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Competitions.DeleteCompetition;

public class DeleteCompetitionCommand : IRequest<Result<DeleteCompetitionResponse>>
{
    public int IdCompetition { get; set; }

    public DeleteCompetitionCommand(int idCompetition)
    {
        IdCompetition = idCompetition;
    }
}
