using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionMetricsController : BaseController
{
    private readonly ISessionMetricRepository _sessionMetricRepository;

    public SessionMetricsController(ISessionMetricRepository sessionMetricRepository)
    {
        _sessionMetricRepository = sessionMetricRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<SessionMetric>> GetAll()
    {
        try
        {
            var sessionMetrics = _sessionMetricRepository.GetAll();
            var result = Result<IEnumerable<SessionMetric>>.Success(sessionMetrics);
            return CreateResponse(result);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving session metrics: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<SessionMetric> GetById(int id)
    {
        try
        {
            var sessionMetric = _sessionMetricRepository.GetById(id);
            if (sessionMetric == null)
            {
                var result = Result.Failure($"Session metric with ID {id} not found").WithCode((int)ResultCode.NotFound);
                return CreateResponse(result);
            }

            var successResult = Result<SessionMetric>.Success(sessionMetric);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving session metric: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpPost]
    public ActionResult<SessionMetric> Create([FromBody] CreateSessionMetricRequest request)
    {
        try
        {
            if (request == null)
            {
                var result = Result.Failure("Session metric data is required").WithCode((int)ResultCode.BadRequest);
                return CreateResponse(result);
            }

            var sessionMetric = new SessionMetric
            {
                Value = request.Value,
                IdSession = request.IdSession,
                IdMetrics = request.IdMetrics
            };

            var createdSessionMetric = _sessionMetricRepository.Create(sessionMetric);
            var successResult = Result<SessionMetric>.Success(createdSessionMetric);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error creating session metric: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }
}

// Request DTOs
public class CreateSessionMetricRequest
{
    public string? Value { get; set; }
    public int IdSession { get; set; }
    public int IdMetrics { get; set; }
}
