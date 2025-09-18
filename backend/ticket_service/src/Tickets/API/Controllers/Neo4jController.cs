using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.API.Controllers;

[ApiController]
[Route("api/neo4j")]
public class Neo4jController : BaseController
{
    private readonly INeo4jSeedingService _neo4jSeedingService;

    public Neo4jController(INeo4jSeedingService neo4jSeedingService)
    {
        _neo4jSeedingService = neo4jSeedingService;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> SeedDatabase()
    {
        var result = await _neo4jSeedingService.SeedDatabaseAndRemoveInitContainer();
        return CreateResponse(result);
    }
}