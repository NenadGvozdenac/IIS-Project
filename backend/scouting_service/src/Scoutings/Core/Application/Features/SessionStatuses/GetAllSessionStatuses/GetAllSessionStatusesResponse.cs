namespace scouting_service.src.Scoutings.Core.Application.Features.SessionStatuses.GetAllSessionStatuses;

public class GetAllSessionStatusesResponse
{
    public List<SessionStatusDto> SessionStatuses { get; set; } = new();
}

public class SessionStatusDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
