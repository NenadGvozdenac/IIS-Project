using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.TravelInformations.GetAllTravelInfos;

public class GetAllTravelInfosHandler : IRequestHandler<GetAllTravelInfosQuery, Result<GetAllTravelInfosResponse>>
{
    private readonly ITravelInfoRepository _travelInfoRepository;

    public GetAllTravelInfosHandler(ITravelInfoRepository travelInfoRepository)
    {
        _travelInfoRepository = travelInfoRepository;
    }

    public Task<Result<GetAllTravelInfosResponse>> Handle(GetAllTravelInfosQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var travelInfos = _travelInfoRepository.GetAll();

            var response = new GetAllTravelInfosResponse
            {
                TravelInfos = travelInfos.Select(t => new TravelInfoDto
                {
                    IdTravelInfo = t.IdTravelInformation,
                    PassportNumber = t.PassportNumber,
                    PassportExpirationDate = t.PassportExpirationDate,
                    Phone = t.Phone,
                    Email = t.Email,
                    Role = t.Role,
                    IdManagementMember = t.IdManagementMember,
                    IdTeam = t.IdTeam,
                    IdPlayer = t.IdPlayer
                }).ToList()
            };

            return Task.FromResult(Result<GetAllTravelInfosResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetAllTravelInfosResponse>.Failure($"An error occurred while retrieving travel information: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}

