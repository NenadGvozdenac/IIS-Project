namespace scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerSessions;

public class GetPlayerSessionsResponse
{
    public int SessionId { get; set; }
    public DateOnly? StartTime { get; set; }
    public DateOnly? EndTime { get; set; }
    public string SessionStatus { get; set; } = null!;
    public string SessionType { get; set; } = null!;
    public string ScoutName { get; set; } = null!;
    public string ScoutSurname { get; set; } = null!;
}