using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Competitions.CreateCompetition;

public class CreateCompetitionCommand : IRequest<Result<CreateCompetitionResponse>>
{
    public string Name { get; set; } = null!;
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
    public int NumberOfMatches { get; set; }
}
