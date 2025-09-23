using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;

namespace scouting_service.src.Scoutings.Core.Application.Features.Players.UpdatePlayer;

public class UpdatePlayerCommand : IRequest<Result<UpdatePlayerResponse>>
{
    public int IdPlayer { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public DateOnly? Birthday { get; set; }
    public int? Weight { get; set; }
    public int? Height { get; set; }
    public int? IdNationality { get; set; }
    public int? IdPosition { get; set; }
}
