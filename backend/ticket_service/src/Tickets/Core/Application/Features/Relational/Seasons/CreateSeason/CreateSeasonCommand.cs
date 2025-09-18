using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Seasons.CreateSeason;

public class CreateSeasonCommand : IRequest<Result<CreateSeasonResponse>>
{
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
    public string Name { get; set; } = null!;
}
