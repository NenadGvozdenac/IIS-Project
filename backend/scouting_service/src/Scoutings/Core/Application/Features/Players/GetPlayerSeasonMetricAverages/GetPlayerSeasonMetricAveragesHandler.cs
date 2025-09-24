using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerSeasonMetricAverages;

public class GetPlayerSeasonMetricAveragesHandler : IRequestHandler<GetPlayerSeasonMetricAveragesQuery, Result<List<GetPlayerSeasonMetricAveragesResponse>>>
{
    private readonly IPlayerRepository _playerRepository;

    public GetPlayerSeasonMetricAveragesHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<Result<List<GetPlayerSeasonMetricAveragesResponse>>> Handle(GetPlayerSeasonMetricAveragesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var metrics = await _playerRepository.GetPlayerSeasonMetricAveragesAsync(
                request.PlayerId, 
                request.SeasonId, 
                request.SessionType
            );

            return Result<List<GetPlayerSeasonMetricAveragesResponse>>.Success(metrics);
        }
        catch (Exception ex)
        {
            return Result<List<GetPlayerSeasonMetricAveragesResponse>>
                .Failure($"An error occurred while retrieving player season metric averages: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}