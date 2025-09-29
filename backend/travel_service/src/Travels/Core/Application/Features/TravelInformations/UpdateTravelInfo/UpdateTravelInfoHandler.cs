using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Features.TravelInformations.UpdateTravelInfo;

public class UpdateTravelInfoHandler : IRequestHandler<UpdateTravelInfoCommand, Result<UpdateTravelInfoResponse>>
{
    private readonly ITravelInfoRepository _travelInfoRepository;
    private readonly IUserRepository _userRepository;

    public UpdateTravelInfoHandler(ITravelInfoRepository travelInfoRepository, IUserRepository userRepository)
    {
        _travelInfoRepository = travelInfoRepository;
        _userRepository = userRepository;
    }

    public Task<Result<UpdateTravelInfoResponse>> Handle(UpdateTravelInfoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingTravelInfo = _travelInfoRepository.GetById(request.Id);
            if (existingTravelInfo == null)
            {
                return Task.FromResult(Result<UpdateTravelInfoResponse>.Failure($"Travel information with ID {request.Id} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var user = _userRepository.GetById(request.UserId);
            if (user == null)
            {
                return Task.FromResult(Result<UpdateTravelInfoResponse>.Failure("User not found")
                    .WithCode((int)ResultCode.NotFound));
            }
            if (user.Type != "team manager")
            {
                return Task.FromResult(Result<UpdateTravelInfoResponse>.Failure("Only team manager can update travel information")
                    .WithCode((int)ResultCode.Forbidden));
            }
            if (string.IsNullOrWhiteSpace(request.PassportNumber))
            {
                return Task.FromResult(Result<UpdateTravelInfoResponse>.Failure("Passport number is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (request.PassportExpirationDate is null)
            {
                return Task.FromResult(Result<UpdateTravelInfoResponse>.Failure("Passport expiration date is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (string.IsNullOrWhiteSpace(request.Phone))
            {
                return Task.FromResult(Result<UpdateTravelInfoResponse>.Failure("Phone is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Task.FromResult(Result<UpdateTravelInfoResponse>.Failure("Email is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (string.IsNullOrWhiteSpace(request.Role))
            {
                return Task.FromResult(Result<UpdateTravelInfoResponse>.Failure("Role is required")
                    .WithCode((int)ResultCode.BadRequest));
            }
            if (request.IdManagementMember is not null)
            {
                if (request.IdTeam is not null || request.IdPlayer is not null)
                {
                    return Task.FromResult(Result<UpdateTravelInfoResponse>.Failure("For management member, IdTeam and IdPlayer must be null")
                        .WithCode((int)ResultCode.BadRequest));
                }
            }
            else if (request.IdTeam is not null && request.IdPlayer is not null)
            {
                if (request.IdManagementMember is not null)
                {
                    return Task.FromResult(Result<UpdateTravelInfoResponse>.Failure("For team and player, IdManagementMember must be null")
                        .WithCode((int)ResultCode.BadRequest));
                }
            }
            else
            {
                return Task.FromResult(Result<UpdateTravelInfoResponse>.Failure("You must provide either IdManagementMember, or both IdTeam and IdPlayer")
                    .WithCode((int)ResultCode.BadRequest));
            }

            existingTravelInfo.PassportNumber = request.PassportNumber;
            existingTravelInfo.PassportExpirationDate = request.PassportExpirationDate;
            existingTravelInfo.Phone = request.Phone;
            existingTravelInfo.Email = request.Email;
            existingTravelInfo.Role = request.Role;
            existingTravelInfo.IdManagementMember = request.IdManagementMember;
            existingTravelInfo.IdTeam = request.IdTeam;
            existingTravelInfo.IdPlayer = request.IdPlayer;

            var updatedTravelInfo = _travelInfoRepository.Update(existingTravelInfo);

            var response = new UpdateTravelInfoResponse
            {
                IdTravelInfo = updatedTravelInfo.IdTravelInformation,
                PassportNumber = updatedTravelInfo.PassportNumber,
                PassportExpirationDate = updatedTravelInfo.PassportExpirationDate,
                Phone = updatedTravelInfo.Phone,
                Email = updatedTravelInfo.Email,
                Role = updatedTravelInfo.Role,
                IdManagementMember = updatedTravelInfo.IdManagementMember,
                IdTeam = updatedTravelInfo.IdTeam,
                IdPlayer = updatedTravelInfo.IdPlayer
            };

            return Task.FromResult(Result<UpdateTravelInfoResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<UpdateTravelInfoResponse>.Failure($"An error occurred while updating the travel information: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}