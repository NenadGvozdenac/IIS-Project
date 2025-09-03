namespace travel_service.src.Travels.Core.Application.Features.ManagementMembers.GetAllMembers;

public class GetAllMembersResponse
{
    public int MemberId { get; set; }

    public string? MemberName { get; set; }

    public string? MemberSurname { get; set; }

    public string? MemberRole { get; set; }
}