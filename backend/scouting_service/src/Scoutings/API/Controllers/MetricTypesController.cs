using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricTypesController : BaseController
{
    private readonly IMetricTypeRepository _metricTypeRepository;

    public MetricTypesController(IMetricTypeRepository metricTypeRepository)
    {
        _metricTypeRepository = metricTypeRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<MetricType>> GetAll()
    {
        try
        {
            var metricTypes = _metricTypeRepository.GetAll();
            var result = Result<IEnumerable<MetricType>>.Success(metricTypes);
            return CreateResponse(result);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving metric types: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<MetricType> GetById(int id)
    {
        try
        {
            var metricType = _metricTypeRepository.GetById(id);
            if (metricType == null)
            {
                var result = Result.Failure($"Metric type with ID {id} not found").WithCode((int)ResultCode.NotFound);
                return CreateResponse(result);
            }

            var successResult = Result<MetricType>.Success(metricType);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving metric type: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpPost]
    public ActionResult<MetricType> Create([FromBody] CreateMetricTypeRequest request)
    {
        try
        {
            if (request == null)
            {
                var result = Result.Failure("Metric type data is required").WithCode((int)ResultCode.BadRequest);
                return CreateResponse(result);
            }

            var metricType = new MetricType
            {
                Type = request.Type
            };

            var createdMetricType = _metricTypeRepository.Create(metricType);
            var successResult = Result<MetricType>.Success(createdMetricType);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error creating metric type: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }
}

// Request DTOs
public class CreateMetricTypeRequest
{
    public string? Type { get; set; }
}
