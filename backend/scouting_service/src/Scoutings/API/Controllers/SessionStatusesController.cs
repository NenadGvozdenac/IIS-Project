using Microsoft.AspNetCore.Mvc;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionStatusesController : BaseController
{
    private readonly ISessionStatusRepository _sessionStatusRepository;

    public SessionStatusesController(ISessionStatusRepository sessionStatusRepository)
    {
        _sessionStatusRepository = sessionStatusRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<SessionStatus>> GetAll()
    {
        try
        {
            var sessionStatuses = _sessionStatusRepository.GetAll();
            var result = Result<IEnumerable<SessionStatus>>.Success(sessionStatuses);
            return CreateResponse(result);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving session statuses: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<SessionStatus> GetById(int id)
    {
        try
        {
            var sessionStatus = _sessionStatusRepository.GetById(id);
            if (sessionStatus == null)
            {
                var result = Result.Failure($"Session status with ID {id} not found").WithCode((int)ResultCode.NotFound);
                return CreateResponse(result);
            }

            var successResult = Result<SessionStatus>.Success(sessionStatus);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error retrieving session status: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }

    [HttpPost]
    public ActionResult<SessionStatus> Create([FromBody] CreateSessionStatusRequest request)
    {
        try
        {
            if (request == null)
            {
                var result = Result.Failure("Session status data is required").WithCode((int)ResultCode.BadRequest);
                return CreateResponse(result);
            }

            var sessionStatus = new SessionStatus
            {
                Status = request.Status
            };

            var createdSessionStatus = _sessionStatusRepository.Create(sessionStatus);
            var successResult = Result<SessionStatus>.Success(createdSessionStatus);
            return CreateResponse(successResult);
        }
        catch (Exception ex)
        {
            var result = Result.Failure($"Error creating session status: {ex.Message}").WithCode((int)ResultCode.BadRequest);
            return CreateResponse(result);
        }
    }
}

// Request DTOs
public class CreateSessionStatusRequest
{
    public string? Status { get; set; }
}
