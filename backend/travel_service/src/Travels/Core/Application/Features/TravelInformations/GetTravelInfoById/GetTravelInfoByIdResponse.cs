namespace travel_service.src.Travels.Core.Application.Features.TravelInformations.GetTravelInfoById;

public class GetTravelInfoByIdResponse
{
    public int IdTravelInformation { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
    public DateOnly? PassportExpirationDate { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int IdManagementMember { get; set; }
    public int IdTeam { get; set; }
    public int IdPlayer { get; set; }
}
