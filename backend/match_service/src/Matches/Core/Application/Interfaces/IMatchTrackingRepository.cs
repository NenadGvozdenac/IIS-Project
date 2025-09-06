using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Interfaces;

public interface IMatchTrackingRepository
{
    MatchTracking? GetByMatchId(int matchId);
    MatchTracking Create(MatchTracking matchTracking);
    MatchTracking? Update(MatchTracking matchTracking);
    bool Delete(int matchId);
    bool Exists(int matchId);
}
