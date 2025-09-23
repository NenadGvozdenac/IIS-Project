using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.Players.GetPlayerById;

public class GetPlayerByIdHandler : IRequestHandler<GetPlayerByIdQuery, Result<GetPlayerByIdResponse>>
{
    private readonly IPlayerRepository _playerRepository;

    public GetPlayerByIdHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public Task<Result<GetPlayerByIdResponse>> Handle(GetPlayerByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var player = _playerRepository.GetById(request.IdPlayer);
            
            if (player == null)
            {
                return Task.FromResult(Result<GetPlayerByIdResponse>.Failure($"Player with ID {request.IdPlayer} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var response = new GetPlayerByIdResponse
            {
                IdPlayer = player.IdPlayer,
                Name = player.Name ?? string.Empty,
                Surname = player.Surname ?? string.Empty,
                Birthday = player.Birthday,
                Weight = player.Weight,
                Height = player.Height,
                IdNationality = player.IdNationality,
                NationalityName = player.IdNationalityNavigation?.State,
                IdPosition = player.IdPosition,
                PositionName = player.IdPositionNavigation?.Name,
                LatestPhysicalMetric = player.PhysicalMetrics
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
            };

            return Task.FromResult(Result<GetPlayerByIdResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetPlayerByIdResponse>.Failure($"An error occurred while retrieving player: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
