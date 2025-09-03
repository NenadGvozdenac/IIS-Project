namespace travel_service.src.Travels.Core.Application.Features.TravelInformations.CreateTravelInfo;

public class CreateTravelInfoResponse
{
    public int IdTravelInfo { get; set; }
    public string? PassportNumber { get; set; }
    public DateOnly? PassportExpirationDate { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public int? IdManagementMember { get; set; }
    public int? IdTeam { get; set; }
    public int? IdPlayer { get; set; }
}