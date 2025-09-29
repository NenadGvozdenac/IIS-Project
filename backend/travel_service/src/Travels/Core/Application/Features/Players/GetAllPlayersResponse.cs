namespace travel_service.src.Travels.Core.Application.Features.Players.GetAllPlayers;

public class GetAllPlayersResponse
{
    public int IdPlayer { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public DateOnly? Birthday { get; set; }

    public int IdNationality { get; set; }

    public int IdPosition { get; set; }
}