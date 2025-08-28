using Microsoft.AspNetCore.Mvc;

namespace match_service.src.Matches.BuildingBlocks.Core.Domain;

public class BaseController : ControllerBase
{

    [NonAction]
    public ActionResult CreateResponse(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return CreateErrorResponse(result);
    }

    [NonAction]
    private ActionResult CreateErrorResponse(Result result)
    {
        return result.Code switch
        {
            ResultCode.BadRequest => BadRequest(result),
            ResultCode.NotFound => NotFound(result),
            ResultCode.Unauthorized => Unauthorized(result),
            ResultCode.Forbidden => Forbid(),
            ResultCode.Conflict => Conflict(result),
            _ => StatusCode(500, result),
        };
    }
}