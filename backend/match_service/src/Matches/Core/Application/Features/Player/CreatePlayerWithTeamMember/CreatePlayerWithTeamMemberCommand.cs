using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.Player.CreatePlayerWithTeamMember;

public class CreatePlayerWithTeamMemberCommand : IRequest<Result<CreatePlayerWithTeamMemberResponse>>
{
    // Player properties
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public DateOnly? Birthday { get; set; }
    public int? Weight { get; set; }
    public int? Height { get; set; }
    public int IdPosition { get; set; }
    
    // TeamMember properties
    public int? JerseyNumber { get; set; }
    public int IdTeam { get; set; }
}
