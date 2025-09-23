namespace scouting_service.src.Scoutings.Core.Application.Features.Players.CreatePlayer;

public class CreatePlayerResponse
{
    public int IdPlayer { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public DateOnly? Birthday { get; set; }
    public int? Weight { get; set; }
    public int? Height { get; set; }
    public int IdNationality { get; set; }
    public int IdPosition { get; set; }
}
