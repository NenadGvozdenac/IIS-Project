namespace match_service.src.Matches.Core.Application.Features.Player.GetPlayerById;

public class GetPlayerByIdResponse
{
    public int IdPlayer { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public DateOnly? Birthday { get; set; }
    public int? Weight { get; set; }
    public int? Height { get; set; }
    public int IdNationality { get; set; }
    public string? NationalityState { get; set; }
    public int IdPosition { get; set; }
    public string? PositionName { get; set; }
}
