namespace travel_service.src.Travels.Core.Application.Features.Visas.GetVisasByTravelInfoId;
public class GetVisasByTravelInfoIdResponse
{
    public List<VisaDTO> Visas { get; set; } = new List<VisaDTO>();
}
public class VisaDTO
{
    public string VisaNumber { get; set; } = null!;
    public string? State { get; set; }
    public DateOnly? CreationDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public int IdTravelInformation { get; set; }
}
