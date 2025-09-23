namespace travel_service.src.Travels.Core.Application.Features.Requests.CheckExistingRequest;

public class CheckExistingRequestResponse
{
    public bool RequestExists { get; set; }
    public int? ExistingRequestId { get; set; }
}