namespace scouting_service.src.Scoutings.Core.Application.Features.Sessions.UpdateSession;

public class UpdateSessionResponse
{
    public int IdSession { get; set; }
    public DateOnly StartTime { get; set; }
    public DateOnly? EndTime { get; set; }
    public int IdSessionStatus { get; set; }
    public int IdSessionType { get; set; }
    public int IdUser { get; set; }
    public int IdPlayer { get; set; }
}