using MediatR;
using Microsoft.AspNetCore.Mvc;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Features.AutomaticRecommendation.CreateRecommendation;
using match_service.src.Matches.Core.Application.Features.AutomaticRecommendation.GetRecommendations;
using match_service.src.Matches.Core.Application.Features.AutomaticRecommendation.GetAllRecommendations;
using match_service.src.Matches.Core.Application.Features.AutomaticRecommendation.AcceptRecommendation;
using match_service.src.Matches.Core.Application.Features.AutomaticRecommendation.RejectRecommendation;

namespace match_service.src.Matches.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutomaticRecommendationController : BaseController
{
    private readonly IMediator _mediator;

    public AutomaticRecommendationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public ActionResult CreateRecommendation([FromBody] CreateRecommendationRequest request)
    {
        var command = new CreateRecommendationCommand
        {
            MatchId = request.MatchId,
            Priority = request.Priority,
            Type = request.Type,
            Description = request.Description
        };
        var result = _mediator.Send(command).Result;
        return CreateResponse(result);
    }

    [HttpGet("match/{matchId}")]
    public ActionResult GetRecommendationsByMatch(int matchId)
    {
        var query = new GetRecommendationsQuery(matchId);
        var result = _mediator.Send(query).Result;
        return CreateResponse(result);
    }

    [HttpGet("match/{matchId}/all")]
    public ActionResult GetAllRecommendationsByMatch(int matchId)
    {
        var query = new GetAllRecommendationsQuery(matchId);
        var result = _mediator.Send(query).Result;
        return CreateResponse(result);
    }

    [HttpPut("{recommendationId}/accept")]
    public ActionResult AcceptRecommendation(int recommendationId)
    {
        var command = new AcceptRecommendationCommand(recommendationId);
        var result = _mediator.Send(command).Result;
        return CreateResponse(result);
    }

    [HttpPut("{recommendationId}/reject")]
    public ActionResult RejectRecommendation(int recommendationId)
    {
        var command = new RejectRecommendationCommand(recommendationId);
        var result = _mediator.Send(command).Result;
        return CreateResponse(result);
    }
}

public class CreateRecommendationRequest
{
    public int MatchId { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}