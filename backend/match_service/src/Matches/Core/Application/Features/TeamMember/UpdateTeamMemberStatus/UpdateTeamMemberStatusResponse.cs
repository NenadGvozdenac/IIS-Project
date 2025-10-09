namespace match_service.src.Matches.Core.Application.Features.TeamMember.UpdateTeamMemberStatus
{
    public class UpdateTeamMemberStatusResponse
    {
        public int IdPlayer { get; set; }
        public int IdTeam { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = "Status updated successfully";
    }
}
