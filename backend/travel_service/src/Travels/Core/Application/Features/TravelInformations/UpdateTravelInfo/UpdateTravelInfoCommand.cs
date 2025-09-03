using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.TravelInformations.UpdateTravelInfo;

public class UpdateTravelInfoCommand : IRequest<Result<UpdateTravelInfoResponse>>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? PassportNumber { get; set; }

    public DateOnly? PassportExpirationDate { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Role { get; set; }

    public int? IdManagementMember { get; set; }

    public int? IdTeam { get; set; }

    public int? IdPlayer { get; set; }
}