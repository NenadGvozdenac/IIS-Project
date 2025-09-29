namespace match_service.src.Matches.Core.Application.Features.TeamMember.DeleteTeamMember;

public class DeleteTeamMemberResponse
{
    public bool IsDeleted { get; set; }
    public string Message { get; set; }
    public Guid? CorrelationId { get; set; }
}
