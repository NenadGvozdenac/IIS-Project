namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Users.GetUserById;

public class GetUserByIdResponse
{
    public int Id_User { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Type { get; set; }
}
