using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.MatchTracking.ResumeMatch;

public class ResumeMatchCommand : IRequest<Result<ResumeMatchResponse>>
{
    public int MatchId { get; set; }
}