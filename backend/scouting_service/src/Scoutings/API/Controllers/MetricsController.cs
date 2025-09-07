using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricsController : BaseController
{
    private readonly IMetricRepository _metricRepository;

    public MetricsController(IMetricRepository metricRepository)
    {
        _metricRepository = metricRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Metric>> GetAll()
    {
        try
        {
            var metrics = _metricRepository.GetAll();
            var result = Result<IEnumerable<Metric>>.Success(metrics);
            return CreateResponse(result);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving metrics: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<Metric> GetById(int id)
    {
        try
        {
            var metric = _metricRepository.GetById(id);
            if (metric == null)
            {
                var result = Result.Failure($"Metric with ID {id} not found").WithCode((int)ResultCode.NotFound);
                return CreateResponse(result);
            }

            var successResult = Result<Metric>.Success(metric);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving metric: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpPost]
    public ActionResult<Metric> Create([FromBody] CreateMetricRequest request)
    {
        try
        {
            if (request == null)
            {
                var result = Result.Failure("Metric data is required").WithCode((int)ResultCode.BadRequest);
                return CreateResponse(result);
            }

            var metric = new Metric
            {
                Name = request.Name,
                IsPermanent = request.IsPermanent,
                MetricWeight = request.MetricWeight,
                IdUser = request.IdUser,
                IdMetricType = request.IdMetricType
            };

            var createdMetric = _metricRepository.Create(metric);
            var successResult = Result<Metric>.Success(createdMetric);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error creating metric: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }
}

// Request DTOs
public class CreateMetricRequest
{
    public string? Name { get; set; }
    public int? IsPermanent { get; set; }
    public int? MetricWeight { get; set; }
    public int IdUser { get; set; }
    public int IdMetricType { get; set; }
}
