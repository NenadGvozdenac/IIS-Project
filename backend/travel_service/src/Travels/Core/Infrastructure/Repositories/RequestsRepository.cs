using Microsoft.EntityFrameworkCore;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;
using travel_service.src.Travels.Core.Infrastructure;
using travel_service.src.Travels.Core.Application.Features.Requests.CreateRequests;

namespace travel_service.src.Travels.Core.Infrastructure.Repositories;

public class RequestsRepository : IRequestsRepository
{
    private readonly TravelDbContext _travelDbContext;

    public RequestsRepository(TravelDbContext context)
    {
        _travelDbContext = context;
    }

    public IEnumerable<Request> GetAllRequests()
    {
        return _travelDbContext.Requests.ToList();
    }

    public IEnumerable<Request> GetRequestsByType(string type)
    {
        return _travelDbContext.Requests
            .Where(a => a.Type == type)
            .ToList();
    }

    public IEnumerable<Request> GetRequestsByTypeWithDetails(string type)
    {
        return _travelDbContext.Requests
            .Include(r => r.AccommodationRequest)
            .Include(r => r.TransportationRequest)
            .Include(r => r.IdManagementMembers)
            .Include(r => r.Ids)
                .ThenInclude(tm => tm.IdPlayerNavigation)
            .Include(r => r.Ids)
                .ThenInclude(tm => tm.IdTeamNavigation)
            .Where(r => r.Type == type)
            .ToList();
    }

    public Request CreateRequest(Request request)
    {
        _travelDbContext.Requests.Add(request);
        _travelDbContext.SaveChanges();
        return request;
    }

    public void AddTeamMembersToRequest(int requestId, List<TeamMemberRequest> teamMemberRequests)
    {
        foreach (var teamMemberRequest in teamMemberRequests)
        {
            _travelDbContext.Database.ExecuteSqlRaw(
                "INSERT INTO team_member_request (id_team, id_player, id_request) VALUES ({0}, {1}, {2})",
                teamMemberRequest.IdTeam, teamMemberRequest.IdPlayer, requestId);
        }
    }

    public void AddManagementMembersToRequest(int requestId, List<int> managementMemberIds)
    {
        foreach (var managementId in managementMemberIds)
        {
            _travelDbContext.Database.ExecuteSqlRaw(
                "INSERT INTO management_member_request (id_request, id_management_member) VALUES ({0}, {1})",
                requestId, managementId);
        }
    }
    public void SendRequestToAgencies(int requestId, List<int> agencyIds)
    {
        foreach (var agencyId in agencyIds)
        {
            var sentRequest = new SentRequest
            {
                IdRequest = requestId,
                IdAgency = agencyId
            };

            _travelDbContext.SentRequests.Add(sentRequest);
        }

        _travelDbContext.SaveChanges();
    }

    public IEnumerable<Request> GetRequestsByMatchId(int matchId)
    {
        return _travelDbContext.Requests
            .Include(r => r.AccommodationRequest)
            .Include(r => r.TransportationRequest)
            .Include(r => r.IdManagementMembers)
            .Include(r => r.Ids)
                .ThenInclude(tm => tm.IdPlayerNavigation)
            .Include(r => r.Ids)
                .ThenInclude(tm => tm.IdTeamNavigation)
            .Where(r => r.IdMatch == matchId)
            .ToList();
    }

    public Request? GetRequestByMatchIdAndType(int matchId, string type)
    {
        return _travelDbContext.Requests
            .Where(r => r.IdMatch == matchId && r.Type == type)
            .FirstOrDefault();
    }

    // Saga deletion method
    public bool DeleteTeamMemberRequests(int playerId, int teamId)
    {
        try
        {
            var rowsAffected = _travelDbContext.Database.ExecuteSqlRaw(
                "DELETE FROM team_member_request WHERE id_player = {0} AND id_team = {1}",
                playerId, teamId);
            
            return rowsAffected > 0;
        }
        catch
        {
            return false;
        }
    }
}
