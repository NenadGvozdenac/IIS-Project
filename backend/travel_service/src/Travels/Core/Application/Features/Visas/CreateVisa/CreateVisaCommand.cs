using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Visas.CreateVisa;

public class CreateVisaCommand : IRequest<Result<CreateVisaResponse>>
{
    public int UserId { get; set; }
    public string VisaNumber { get; set; } = null!;

    public string? State { get; set; }

    public DateOnly? CreationDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public int IdTravelInformation { get; set; }
}