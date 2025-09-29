using travel_service.src.Travels.Core.Application.Features.Requests.GetAllRequests;

namespace travel_service.src.Travels.Core.Application.Features.Requests.GetRequestsByMatch;

public class GetRequestsByMatchResponse
{
    public List<RequestDto> Requests { get; set; } = new();
}