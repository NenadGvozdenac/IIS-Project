using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Features.Requests.GetAllRequests;

public class GetAllRequestsHandler : IRequestHandler<GetAllRequestsQuery, Result<GetAllRequestsResponse>>
{
    private readonly IRequestsRepository _requestsRepository;

    public GetAllRequestsHandler(IRequestsRepository requestsRepository)
    {
        _requestsRepository = requestsRepository;
    }

    public Task<Result<GetAllRequestsResponse>> Handle(GetAllRequestsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var requests = _requestsRepository.GetRequestsByTypeWithDetails(request.Type);
            var response = new GetAllRequestsResponse
            {
                Requests = requests.Select(r => new RequestDto
                {
                    IdRequest = r.IdRequest,
                    State = r.State,
                    City = r.City,
                    Hall = r.Hall,
                    Budget = r.Budget,
                    IdMatch = r.IdMatch,
                    Type = r.Type,
                    
                    NumberOfGuests = r.AccommodationRequest?.NumberOfGuests,
                    NumberOfRooms = r.AccommodationRequest?.NumberOfRooms,
                    CheckInDate = r.AccommodationRequest?.CheckInDate,
                    CheckOutDate = r.AccommodationRequest?.CheckOutDate,
                    AccommodationType = r.AccommodationRequest?.AccommodationType,
                    
                    NumberOfPassengers = r.TransportationRequest?.NumberOfPassengers,
                    StartDate = r.TransportationRequest?.StartDate,
                    EndDate = r.TransportationRequest?.EndDate,
                    VehicleType = r.TransportationRequest?.VehicleType,

                    TeamMembers = r.Ids.Select(tm => new TeamMemberDto
                    {
                        IdTeam = tm.IdTeam,
                        IdPlayer = tm.IdPlayer,
                        PlayerName = tm.IdPlayerNavigation?.Name ?? "",
                        PlayerSurname = tm.IdPlayerNavigation?.Surname ?? "",
                        TeamName = tm.IdTeamNavigation?.Name ?? "",
                        JerseyNumber = tm.JerseyNumber,
                        Status = tm.Status ?? ""
                    }).ToList(),

                    ManagementMembers = r.IdManagementMembers.Select(m => new ManagementMemberDto
                    {
                        MemberId = m.MemberId,
                        MemberName = m.MemberName ?? "",
                        MemberSurname = m.MemberSurname ?? "",
                        MemberRole = m.MemberRole ?? ""
                    }).ToList()
                }).ToList()
            };

            return Task.FromResult(Result<GetAllRequestsResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetAllRequestsResponse>.Failure($"An error occurred while retrieving requests: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}