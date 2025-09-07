namespace scouting_service.src.Scoutings.Core.Application.Features.SessionTypes.GetAllSessionTypes;

public class GetAllSessionTypesResponse
{
    public List<SessionTypeDto> SessionTypes { get; set; } = new();
}

public class SessionTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
