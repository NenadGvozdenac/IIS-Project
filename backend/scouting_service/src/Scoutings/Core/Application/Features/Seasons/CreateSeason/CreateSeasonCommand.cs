using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Seasons.CreateSeason;

public class CreateSeasonCommand : IRequest<Result<CreateSeasonResponse>>
{
    public string? Name { get; set; }
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
}
