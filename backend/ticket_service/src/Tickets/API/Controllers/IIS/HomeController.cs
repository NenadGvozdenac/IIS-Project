using Microsoft.AspNetCore.Mvc;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

namespace ticket_service.src.Tickets.API.Controllers.IIS;

[ApiController]
[Route("api/home")]
public class HomeController : BaseController
{
    [HttpGet]
    public IActionResult Get()
    {
        return CreateResponse(Result<object>.Success(new
        {
            message = "Welcome to Ticket Service API"
        }));
    }
}
