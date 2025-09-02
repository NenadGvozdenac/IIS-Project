using MediatR;
using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Features.Competitions.GetAllCompetitions;
using ticket_service.src.Tickets.Core.Application.Features.Competitions.GetCompetitionById;
using ticket_service.src.Tickets.Core.Application.Features.Competitions.CreateCompetition;
using ticket_service.src.Tickets.Core.Application.Features.Competitions.UpdateCompetition;
using ticket_service.src.Tickets.Core.Application.Features.Competitions.DeleteCompetition;
using ticket_service.src.Tickets.Core.Application.Features.Competitions.GetActiveCompetitions;

namespace ticket_service.src.Tickets.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompetitionsController : BaseController
{
    private readonly IMediator _mediator;

    public CompetitionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all competitions
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAllCompetitions()
    {
        var query = new GetAllCompetitionsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    /// <summary>
    /// Get competition by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetCompetitionById(int id)
    {
        var query = new GetCompetitionByIdQuery(id);
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    /// <summary>
    /// Get active competitions only
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult> GetActiveCompetitions()
    {
        var query = new GetActiveCompetitionsQuery();
        var result = await _mediator.Send(query);
        return CreateResponse(result);
    }

    /// <summary>
    /// Create a new competition
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateCompetition([FromBody] CreateCompetitionRequest request)
    {
        var command = new CreateCompetitionCommand
        {
            Name = request.Name,
            StartedAt = request.StartedAt,
            EndedAt = request.EndedAt,
            NumberOfMatches = request.NumberOfMatches
        };

        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    /// <summary>
    /// Update an existing competition
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCompetition(int id, [FromBody] UpdateCompetitionRequest request)
    {
        var command = new UpdateCompetitionCommand
        {
            IdCompetition = id,
            Name = request.Name,
            StartedAt = request.StartedAt,
            EndedAt = request.EndedAt,
            NumberOfMatches = request.NumberOfMatches
        };

        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }

    /// <summary>
    /// Delete a competition
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCompetition(int id)
    {
        var command = new DeleteCompetitionCommand(id);
        var result = await _mediator.Send(command);
        return CreateResponse(result);
    }
}

// Request DTOs
public class CreateCompetitionRequest
{
    public string Name { get; set; } = null!;
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
    public int NumberOfMatches { get; set; }
}

public class UpdateCompetitionRequest
{
    public string Name { get; set; } = null!;
    public DateOnly StartedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
    public int NumberOfMatches { get; set; }
}
