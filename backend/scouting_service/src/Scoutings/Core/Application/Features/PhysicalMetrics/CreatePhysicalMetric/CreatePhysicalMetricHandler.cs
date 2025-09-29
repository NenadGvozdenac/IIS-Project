using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Features.PhysicalMetrics.CreatePhysicalMetric;

public class CreatePhysicalMetricHandler : IRequestHandler<CreatePhysicalMetricCommand, Result<CreatePhysicalMetricResponse>>
{
    private readonly IPhysicalMetricRepository _physicalMetricRepository;

    public CreatePhysicalMetricHandler(IPhysicalMetricRepository physicalMetricRepository)
    {
        _physicalMetricRepository = physicalMetricRepository;
    }

    public Task<Result<CreatePhysicalMetricResponse>> Handle(CreatePhysicalMetricCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var physicalMetric = new PhysicalMetric
            {
                VerticalJump = request.VerticalJump,
                FatPercentage = request.FatPercentage,
                BenchPressWeight = request.BenchPressWeight,
                SquatWeight = request.SquatWeight,
                SprintSpeed = request.SprintSpeed,
                Weight = request.Weight,
                Height = request.Height,
                Wingspan = request.Wingspan,
                DateOfMeasurement = request.DateOfMeasurement,
                IdPlayer = request.IdPlayer
            };

            var createdPhysicalMetric = _physicalMetricRepository.Create(physicalMetric);

            var response = new CreatePhysicalMetricResponse
            {
                IdPhysicalMetrics = createdPhysicalMetric.IdPhysicalMetrics,
                VerticalJump = createdPhysicalMetric.VerticalJump,
                FatPercentage = createdPhysicalMetric.FatPercentage,
                BenchPressWeight = createdPhysicalMetric.BenchPressWeight,
                SquatWeight = createdPhysicalMetric.SquatWeight,
                SprintSpeed = createdPhysicalMetric.SprintSpeed,
                Weight = createdPhysicalMetric.Weight,
                Height = createdPhysicalMetric.Height,
                Wingspan = createdPhysicalMetric.Wingspan,
                DateOfMeasurement = createdPhysicalMetric.DateOfMeasurement,
                IdPlayer = createdPhysicalMetric.IdPlayer
            };

            return Task.FromResult(Result<CreatePhysicalMetricResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreatePhysicalMetricResponse>.Failure($"An error occurred while creating physical metric: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
