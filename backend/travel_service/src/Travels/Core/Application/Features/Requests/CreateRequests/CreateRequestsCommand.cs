using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Requests.CreateRequests;

public class TeamMemberRequest
{
    public int IdTeam { get; set; }
    public int IdPlayer { get; set; }
}

public class CreateRequestsCommand : IRequest<Result<CreateRequestsResponse>>
{
    public int UserId { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? Hall { get; set; }
    public int? Budget { get; set; }
    public int IdMatch { get; set; }
    public string Type { get; set; } = string.Empty;
    
    public int? NumberOfGuests { get; set; }
    public int? NumberOfRooms { get; set; }
    public DateOnly? CheckInDate { get; set; }
    public DateOnly? CheckOutDate { get; set; }
    public string? AccommodationType { get; set; }
    
    public int? NumberOfPassengers { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? VehicleType { get; set; }

    public List<TeamMemberRequest> TeamMemberRequests { get; set; } = new List<TeamMemberRequest>();
    public List<int> ManagementMemberIds { get; set; } = new List<int>();
    public List<int> AgencyIds { get; set; } = new List<int>();
}