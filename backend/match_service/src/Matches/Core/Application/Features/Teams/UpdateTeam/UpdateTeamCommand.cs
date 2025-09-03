using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.Teams.UpdateTeam;

public class UpdateTeamCommand : IRequest<Result<UpdateTeamResponse>>
{
    public int IdTeam { get; set; }
    public string? Name { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? Hall { get; set; }
    public DateOnly? FoundedDate { get; set; }
    public string? Coach { get; set; }
    public string? PlayingStyle { get; set; }
    public string? KeyStrengths { get; set; }
    public string? KeyWeaknesses { get; set; }
}
