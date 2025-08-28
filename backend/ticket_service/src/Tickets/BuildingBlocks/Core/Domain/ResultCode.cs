namespace ticket_service.src.Tickets.BuildingBlocks.Core.Domain;

public enum ResultCode
{
    Success = 200,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    Conflict = 409,
    InternalServerError = 500
}