using travel_service.src.Travels.Core.Domain.Entities;
using travel_service.src.Travels.Core.Application.Features.Requests.CreateRequests;

namespace travel_service.src.Travels.Core.Application.Interfaces;

public interface IRequestsRepository
{
    IEnumerable<Request> GetAllRequests();
    IEnumerable<Request> GetRequestsByType(string type);
    IEnumerable<Request> GetRequestsByTypeWithDetails(string type);
    IEnumerable<Request> GetRequestsByMatchId(int matchId);
    Request? GetRequestByMatchIdAndType(int matchId, string type);
    Request CreateRequest(Request request);
    void AddTeamMembersToRequest(int requestId, List<TeamMemberRequest> teamMemberRequests);
    void AddManagementMembersToRequest(int requestId, List<int> managementMemberIds);
    void SendRequestToAgencies(int requestId, List<int> agencyIds);
}