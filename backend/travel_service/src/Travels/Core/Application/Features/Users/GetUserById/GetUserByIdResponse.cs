namespace travel_service.src.Travels.Core.Application.Features.Users.GetUserById;

public class GetUserByIdResponse
{
    public int Id_User { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Type { get; set; }
}
