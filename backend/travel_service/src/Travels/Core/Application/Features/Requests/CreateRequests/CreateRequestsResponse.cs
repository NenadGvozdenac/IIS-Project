namespace travel_service.src.Travels.Core.Application.Features.Requests.CreateRequests;

public class CreateRequestsResponse
{
    public int IdRequest { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? Hall { get; set; }
    public int? Budget { get; set; }
    public int IdMatch { get; set; }
    public string Type { get; set; } = string.Empty;
    public List<TeamMemberRequest> TeamMemberRequests { get; set; } = new List<TeamMemberRequest>();
    public List<int> ManagementMemberIds { get; set; } = new List<int>();
}