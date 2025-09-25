namespace scouting_service.src.Scoutings.Core.Application.Features.Sessions.GetSessionById;

public class GetSessionByIdResponse
{
    public int IdSession { get; set; }
    public DateOnly? StartTime { get; set; }
    public DateOnly? EndTime { get; set; }
    public int IdSessionStatus { get; set; }
    public string? SessionStatusName { get; set; }
    public int IdSessionType { get; set; }
    public string? SessionTypeName { get; set; }
    public int IdUser { get; set; }
    public string? UserName { get; set; }
    public int IdPlayer { get; set; }
    public string? PlayerName { get; set; }
}
