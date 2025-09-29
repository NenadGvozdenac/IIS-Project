using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerSessions;

public class GetPlayerSessionsHandler : IRequestHandler<GetPlayerSessionsQuery, Result<List<GetPlayerSessionsResponse>>>
{
    private readonly IPlayerRepository _playerRepository;

    public GetPlayerSessionsHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<Result<List<GetPlayerSessionsResponse>>> Handle(GetPlayerSessionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var sessions = await _playerRepository.GetPlayerSessionsAsync(
                request.PlayerId, 
                request.SeasonId, 
                request.Status, 
                request.DateFrom, 
                request.DateTo
            );

            return Result<List<GetPlayerSessionsResponse>>.Success(sessions);
        }
        catch (Exception ex)
        {
            return Result<List<GetPlayerSessionsResponse>>
                .Failure($"An error occurred while retrieving player sessions: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}