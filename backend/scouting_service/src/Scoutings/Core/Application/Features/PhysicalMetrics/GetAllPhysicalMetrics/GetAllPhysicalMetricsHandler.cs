using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.PhysicalMetrics.GetAllPhysicalMetrics;

public class GetAllPhysicalMetricsHandler : IRequestHandler<GetAllPhysicalMetricsQuery, Result<List<GetAllPhysicalMetricsResponse>>>
{
    private readonly IPhysicalMetricRepository _physicalMetricRepository;

    public GetAllPhysicalMetricsHandler(IPhysicalMetricRepository physicalMetricRepository)
    {
        _physicalMetricRepository = physicalMetricRepository;
    }

    public Task<Result<List<GetAllPhysicalMetricsResponse>>> Handle(GetAllPhysicalMetricsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var physicalMetrics = _physicalMetricRepository.GetAll();

            var response = physicalMetrics.Select(pm => new GetAllPhysicalMetricsResponse
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
                IdPlayer = pm.IdPlayer,
                PlayerName = pm.IdPlayerNavigation != null ? $"{pm.IdPlayerNavigation.Name} {pm.IdPlayerNavigation.Surname}" : null
            }).ToList();

            return Task.FromResult(Result<List<GetAllPhysicalMetricsResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetAllPhysicalMetricsResponse>>.Failure($"An error occurred while retrieving physical metrics: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
