using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Features.Requests.CreateRequests;

public class CreateRequestsHandler : IRequestHandler<CreateRequestsCommand, Result<CreateRequestsResponse>>
{
    private readonly IRequestsRepository _requestsRepository;
    private readonly IUserRepository _userRepository;

    public CreateRequestsHandler(IRequestsRepository requestsRepository, IUserRepository userRepository)
    {
        _requestsRepository = requestsRepository;
        _userRepository = userRepository;
    }

    public Task<Result<CreateRequestsResponse>> Handle(CreateRequestsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = _userRepository.GetById(request.UserId);
            if (user == null)
            {
                return Task.FromResult(Result<CreateRequestsResponse>.Failure("User not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            if (user.Type != "team manager")
            {
                return Task.FromResult(Result<CreateRequestsResponse>.Failure("Only team manager can create requests")
                    .WithCode((int)ResultCode.Forbidden));
            }

            if (string.IsNullOrWhiteSpace(request.Type) || 
                (request.Type != "accommodation" && request.Type != "transportation"))
            {
                return Task.FromResult(Result<CreateRequestsResponse>.Failure("Type must be 'accommodation' or 'transportation'")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Kreiranje osnovnog Request-a
            var newRequest = new Request
            {
                State = request.State,
                City = request.City,
                Hall = request.Hall,
                Budget = request.Budget,
                IdMatch = request.IdMatch,
                Type = request.Type
            };

            if (request.Type == "accommodation")
            {
                if (request.NumberOfGuests == null || request.CheckInDate == null || request.CheckOutDate == null)
                {
                    return Task.FromResult(Result<CreateRequestsResponse>.Failure("NumberOfGuests, CheckInDate and CheckOutDate are required for accommodation requests")
                        .WithCode((int)ResultCode.BadRequest));
                }

                newRequest.AccommodationRequest = new AccommodationRequest
                {
                    NumberOfGuests = request.NumberOfGuests,
                    NumberOfRooms = request.NumberOfRooms,
                    CheckInDate = request.CheckInDate,
                    CheckOutDate = request.CheckOutDate,
                    AccommodationType = request.AccommodationType
                };
            }
            else if (request.Type == "transportation")
            {
                if (request.NumberOfPassengers == null || request.StartDate == null || request.EndDate == null)
                {
                    return Task.FromResult(Result<CreateRequestsResponse>.Failure("NumberOfPassengers, StartDate and EndDate are required for transportation requests")
                        .WithCode((int)ResultCode.BadRequest));
                }

                newRequest.TransportationRequest = new TransportationRequest
                {
                    NumberOfPassengers = request.NumberOfPassengers,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    VehicleType = request.VehicleType
                };
            }

            var createdRequest = _requestsRepository.CreateRequest(newRequest);

            _requestsRepository.AddTeamMembersToRequest(createdRequest.IdRequest, request.TeamMemberRequests);
            _requestsRepository.AddManagementMembersToRequest(createdRequest.IdRequest, request.ManagementMemberIds);
            _requestsRepository.SendRequestToAgencies(createdRequest.IdRequest, request.AgencyIds);
            

            var response = new CreateRequestsResponse
            {
                IdRequest = createdRequest.IdRequest,
                State = createdRequest.State,
                City = createdRequest.City,
                Hall = createdRequest.Hall,
                Budget = createdRequest.Budget,
                IdMatch = createdRequest.IdMatch,
                Type = createdRequest.Type,
                TeamMemberRequests = new List<TeamMemberRequest>(), // Ovo možeš popuniti ako treba
                ManagementMemberIds = new List<int>() // Ovo možeš popuniti ako treba
            };

            return Task.FromResult(Result<CreateRequestsResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreateRequestsResponse>.Failure($"An error occurred while creating the request: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}