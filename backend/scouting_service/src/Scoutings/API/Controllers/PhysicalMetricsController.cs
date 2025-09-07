using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhysicalMetricsController : BaseController
{
    private readonly IPhysicalMetricRepository _physicalMetricRepository;

    public PhysicalMetricsController(IPhysicalMetricRepository physicalMetricRepository)
    {
        _physicalMetricRepository = physicalMetricRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PhysicalMetric>> GetAll()
    {
        try
        {
            var physicalMetrics = _physicalMetricRepository.GetAll();
            var result = Result<IEnumerable<PhysicalMetric>>.Success(physicalMetrics);
            return CreateResponse(result);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving physical metrics: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<PhysicalMetric> GetById(int id)
    {
        try
        {
            var physicalMetric = _physicalMetricRepository.GetById(id);
            if (physicalMetric == null)
            {
                var result = Result.Failure($"Physical metric with ID {id} not found").WithCode((int)ResultCode.NotFound);
                return CreateResponse(result);
            }

            var successResult = Result<PhysicalMetric>.Success(physicalMetric);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving physical metric: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpPost]
    public ActionResult<PhysicalMetric> Create([FromBody] CreatePhysicalMetricRequest request)
    {
        try
        {
            if (request == null)
            {
                var result = Result.Failure("Physical metric data is required").WithCode((int)ResultCode.BadRequest);
                return CreateResponse(result);
            }

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
            var successResult = Result<PhysicalMetric>.Success(createdPhysicalMetric);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error creating physical metric: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }
}

// Request DTOs
public class CreatePhysicalMetricRequest
{
    public int? VerticalJump { get; set; }
    public int? FatPercentage { get; set; }
    public int? BenchPressWeight { get; set; }
    public int? SquatWeight { get; set; }
    public int? SprintSpeed { get; set; }
    public int? Weight { get; set; }
    public int? Height { get; set; }
    public int? Wingspan { get; set; }
    public DateOnly? DateOfMeasurement { get; set; }
    public int IdPlayer { get; set; }
}
