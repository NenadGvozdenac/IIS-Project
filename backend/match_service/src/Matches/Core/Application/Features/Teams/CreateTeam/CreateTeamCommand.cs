using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.Teams.CreateTeam;

public class CreateTeamCommand : IRequest<Result<CreateTeamResponse>>
{
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
