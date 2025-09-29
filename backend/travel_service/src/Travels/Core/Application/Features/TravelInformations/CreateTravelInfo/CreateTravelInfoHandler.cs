using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Features.TravelInformations.CreateTravelInfo;

public class CreateTravelInfoHandler : IRequestHandler<CreateTravelInfoCommand, Result<CreateTravelInfoResponse>>
{
    private readonly ITravelInfoRepository _travelInfoRepository;
    private readonly IUserRepository _userRepository;

    public CreateTravelInfoHandler(ITravelInfoRepository travelInfoRepository, IUserRepository userRepository)
    {
        _travelInfoRepository = travelInfoRepository;
        _userRepository = userRepository;
    }

    public Task<Result<CreateTravelInfoResponse>> Handle(CreateTravelInfoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = _userRepository.GetById(request.UserId);
            if (user == null)
            {
                return Task.FromResult(Result<CreateTravelInfoResponse>.Failure("User not found")
                    .WithCode((int)ResultCode.NotFound));
            }
            if (user.Type != "team manager")
            {
                return Task.FromResult(Result<CreateTravelInfoResponse>.Failure("Only team manager can create travel information")
                    .WithCode((int)ResultCode.Forbidden));
            }

            if (string.IsNullOrWhiteSpace(request.PassportNumber))
            {
                return Task.FromResult(Result<CreateTravelInfoResponse>.Failure("Passport number is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (request.PassportExpirationDate is null)
            {
                return Task.FromResult(Result<CreateTravelInfoResponse>.Failure("Passport expiration date is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (string.IsNullOrWhiteSpace(request.Phone))
            {
                return Task.FromResult(Result<CreateTravelInfoResponse>.Failure("Phone is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Task.FromResult(Result<CreateTravelInfoResponse>.Failure("Email is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (string.IsNullOrWhiteSpace(request.Role))
            {
                return Task.FromResult(Result<CreateTravelInfoResponse>.Failure("Role is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (request.IdManagementMember is not null)
            {
                if (request.IdTeam is not null || request.IdPlayer is not null)
                {
                    return Task.FromResult(Result<CreateTravelInfoResponse>.Failure("For management member, IdTeam and IdPlayer must be null")
                        .WithCode((int)ResultCode.BadRequest));
                }
            }
            else if (request.IdTeam is not null && request.IdPlayer is not null)
            {
                if (request.IdManagementMember is not null)
                {
                    return Task.FromResult(Result<CreateTravelInfoResponse>.Failure("For team and player, IdManagementMember must be null")
                        .WithCode((int)ResultCode.BadRequest));
                }
            }
            else
            {
                return Task.FromResult(Result<CreateTravelInfoResponse>.Failure("You must provide either IdManagementMember, or both IdTeam and IdPlayer")
                    .WithCode((int)ResultCode.BadRequest));
            }
            var travelInfo = new TravelInformation
            {
                PassportNumber = request.PassportNumber,
                PassportExpirationDate = request.PassportExpirationDate,
                Phone = request.Phone,
                Email = request.Email,
                Role = request.Role,
                IdManagementMember = request.IdManagementMember,
                IdTeam = request.IdTeam,
                IdPlayer = request.IdPlayer
            };

            var createdTravelInfo = _travelInfoRepository.Create(travelInfo);

            var response = new CreateTravelInfoResponse
            {
                IdTravelInfo = createdTravelInfo.IdTravelInformation,
                PassportNumber = createdTravelInfo.PassportNumber,
                PassportExpirationDate = createdTravelInfo.PassportExpirationDate,
                Phone = createdTravelInfo.Phone,
                Email = createdTravelInfo.Email,
                Role = createdTravelInfo.Role,
                IdManagementMember = createdTravelInfo.IdManagementMember,
                IdTeam = createdTravelInfo.IdTeam,
                IdPlayer = createdTravelInfo.IdPlayer
            };

            return Task.FromResult(Result<CreateTravelInfoResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreateTravelInfoResponse>.Failure($"An error occurred while creating the travel information: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}