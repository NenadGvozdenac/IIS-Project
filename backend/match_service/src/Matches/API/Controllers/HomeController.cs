using Microsoft.AspNetCore.Mvc;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.API.Controllers;

[ApiController]
[Route("api/home")]
public class HomeController : BaseController
{
    [HttpGet]
    public IActionResult Get()
    {
        return CreateResponse(Result<object>.Success(new
        {
            message = "Welcome to Match Service API"
        }));
    }
}
