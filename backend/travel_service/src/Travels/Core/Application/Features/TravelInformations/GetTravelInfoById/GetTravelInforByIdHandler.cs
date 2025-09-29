using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.TravelInformations.GetTravelInfoById;

public class GetTravelInfoByIdHandler : IRequestHandler<GetTravelInfoByIdQuery, Result<GetTravelInfoByIdResponse>>
{
    private readonly ITravelInfoRepository _travelInfoRepository;

    public GetTravelInfoByIdHandler(ITravelInfoRepository travelInfoRepository)
    {
        _travelInfoRepository = travelInfoRepository;
    }

    public Task<Result<GetTravelInfoByIdResponse>> Handle(GetTravelInfoByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var travelInfo = _travelInfoRepository.GetById(request.Id);

            if (travelInfo == null)
            {
                return Task.FromResult(Result<GetTravelInfoByIdResponse>.Failure($"Travel information with ID {request.Id} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var response = new GetTravelInfoByIdResponse
            {
                IdTravelInformation = travelInfo.IdTravelInformation,
                PassportNumber = travelInfo.PassportNumber ?? string.Empty,
                PassportExpirationDate = travelInfo.PassportExpirationDate,
                Phone = travelInfo.Phone ?? string.Empty,
                Email = travelInfo.Email ?? string.Empty,
                Role = travelInfo.Role ?? string.Empty,
                IdManagementMember = travelInfo.IdManagementMember.HasValue ? travelInfo.IdManagementMember.Value : 0,
                IdTeam = travelInfo.IdTeam.HasValue ? travelInfo.IdTeam.Value : 0,
                IdPlayer = travelInfo.IdPlayer.HasValue ? travelInfo.IdPlayer.Value : 0
            };

            return Task.FromResult(Result<GetTravelInfoByIdResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetTravelInfoByIdResponse>.Failure($"An error occurred while retrieving the travel information: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
