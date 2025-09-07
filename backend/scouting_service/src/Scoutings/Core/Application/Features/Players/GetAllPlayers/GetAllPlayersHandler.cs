using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.Players.GetAllPlayers;

public class GetAllPlayersHandler : IRequestHandler<GetAllPlayersQuery, Result<List<GetAllPlayersResponse>>>
{
    private readonly IPlayerRepository _playerRepository;

    public GetAllPlayersHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public Task<Result<List<GetAllPlayersResponse>>> Handle(GetAllPlayersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var players = _playerRepository.GetAll();

            var response = players.Select(p => new GetAllPlayersResponse
            {
                IdPlayer = p.IdPlayer,
                Name = p.Name ?? string.Empty,
                Surname = p.Surname ?? string.Empty,
                Birthday = p.Birthday,
                Weight = p.Weight,
                Height = p.Height,
                IdNationality = p.IdNationality,
                NationalityName = p.IdNationalityNavigation?.State,
                IdPosition = p.IdPosition,
                PositionName = p.IdPositionNavigation?.Name,
                LatestPhysicalMetric = p.PhysicalMetrics
                    .OrderByDescending(pm => pm.DateOfMeasurement)
                    .Select(pm => new PhysicalMetricResponse
                    {
                        IdPhysicalMetrics = pm.IdPhysicalMetrics,
                        VerticalJump = pm.VerticalJump,
                        FatPercentage = pm.FatPercentage,
                        BenchPressWeight = pm.BenchPressWeight,
                        SquatWeight = pm.SquatWeight,
                        SprintSpeed = pm.SprintSpeed,
                        Weight = pm.Weight,
                        Height = pm.Height,
                        Wingspan = pm.Wingspan,
                        DateOfMeasurement = pm.DateOfMeasurement,
                        IdPlayer = pm.IdPlayer
                    }).FirstOrDefault()
            }).ToList();

            return Task.FromResult(Result<List<GetAllPlayersResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetAllPlayersResponse>>.Failure($"An error occurred while retrieving players: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
