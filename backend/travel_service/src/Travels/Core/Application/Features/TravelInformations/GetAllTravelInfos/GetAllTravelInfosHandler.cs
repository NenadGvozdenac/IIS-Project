using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.TravelInformations.GetAllTravelInfos;

public class GetAllTravelInfosHandler : IRequestHandler<GetAllTravelInfosQuery, Result<GetAllTravelInfosResponse>>
{
    private readonly ITravelInfoRepository _travelInfoRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly INationalityRepository _nationalityRepository;

    public GetAllTravelInfosHandler(ITravelInfoRepository travelInfoRepository, IPlayerRepository playerRepository, INationalityRepository nationalityRepository)
    {
        _travelInfoRepository = travelInfoRepository;
        _playerRepository = playerRepository;
        _nationalityRepository = nationalityRepository;
    }

    public Task<Result<GetAllTravelInfosResponse>> Handle(GetAllTravelInfosQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var travelInfos = _travelInfoRepository.GetAll();

            var response = new GetAllTravelInfosResponse
            {
                TravelInfos = travelInfos.Select(t => {
                    var player = t.IdPlayer.HasValue ? _playerRepository.GetById(t.IdPlayer.Value) : null;
                    var nationality = player != null
                        ? _nationalityRepository.GetById(player.IdNationality).State ?? string.Empty
                        : null;
                    return new TravelInfoDto
                    {
                        IdTravelInfo = t.IdTravelInformation,
                        PassportNumber = t.PassportNumber,
                        PassportExpirationDate = t.PassportExpirationDate,
                        Phone = t.Phone,
                        Email = t.Email,
                        Role = t.Role,
                        IdManagementMember = t.IdManagementMember,
                        IdTeam = t.IdTeam,
                        IdPlayer = t.IdPlayer,
                        FullPlayerName = player != null ? $"{player.Name} {player.Surname}" : string.Empty,
                        Nationality = nationality ?? string.Empty
                    };
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

