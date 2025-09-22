namespace travel_service.src.Travels.Core.Application.Features.Requests.GetAllRequests;

public class GetAllRequestsResponse
{
    public List<RequestDto> Requests { get; set; } = new List<RequestDto>();
}

public class TeamMemberDto
{
    public int IdTeam { get; set; }
    public int IdPlayer { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string PlayerSurname { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public int? JerseyNumber { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ManagementMemberDto
{
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string MemberSurname { get; set; } = string.Empty;
    public string MemberRole { get; set; } = string.Empty;
}

public class RequestDto
{
    public int IdRequest { get; set; }
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

    public List<TeamMemberDto> TeamMembers { get; set; } = new List<TeamMemberDto>();
    public List<ManagementMemberDto> ManagementMembers { get; set; } = new List<ManagementMemberDto>();
}


