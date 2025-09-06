using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Domain.Entities;
using match_service.src.Matches.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace match_service.src.Matches.Core.Infrastructure.Repositories;

public class MatchTrackingRepository : IMatchTrackingRepository
{
    private readonly MatchDbContext _context;

    public MatchTrackingRepository(MatchDbContext context)
    {
        _context = context;
    }

    public MatchTracking? GetByMatchId(int matchId)
    {
        return _context.MatchTrackings
            .Include(mt => mt.IdMatchNavigation)
            .Include(mt => mt.IdUserNavigation)
            .FirstOrDefault(mt => mt.IdMatch == matchId);
    }

    public MatchTracking Create(MatchTracking matchTracking)
    {
        _context.MatchTrackings.Add(matchTracking);
        _context.SaveChanges();
        return matchTracking;
    }

    public MatchTracking? Update(MatchTracking matchTracking)
    {
        var existingMatchTracking = _context.MatchTrackings.FirstOrDefault(mt => mt.IdMatch == matchTracking.IdMatch);
        if (existingMatchTracking == null)
            return null;

        existingMatchTracking.StartTime = matchTracking.StartTime;
        existingMatchTracking.EndTime = matchTracking.EndTime;
        existingMatchTracking.TrackingStatus = matchTracking.TrackingStatus;
        existingMatchTracking.PeriodDuration = matchTracking.PeriodDuration;
        existingMatchTracking.CurrentPeriod = matchTracking.CurrentPeriod;
        existingMatchTracking.PeriodStatus = matchTracking.PeriodStatus;
        existingMatchTracking.PeriodStartTime = matchTracking.PeriodStartTime;
        existingMatchTracking.ElapsedPeriodTime = matchTracking.ElapsedPeriodTime;
        existingMatchTracking.LastPauseStartTime = matchTracking.LastPauseStartTime;
        existingMatchTracking.TotalPauseTimeInPeriod = matchTracking.TotalPauseTimeInPeriod;
        existingMatchTracking.LastUpdateTime = matchTracking.LastUpdateTime;
        existingMatchTracking.OurPoints = matchTracking.OurPoints;
        existingMatchTracking.OpponentPoints = matchTracking.OpponentPoints;
        existingMatchTracking.IdUser = matchTracking.IdUser;

        _context.SaveChanges();
        return existingMatchTracking;
    }

    public bool Delete(int matchId)
    {
        var matchTracking = _context.MatchTrackings.FirstOrDefault(mt => mt.IdMatch == matchId);
        if (matchTracking == null)
            return false;

        _context.MatchTrackings.Remove(matchTracking);
        _context.SaveChanges();
        return true;
    }

    public bool Exists(int matchId)
    {
        return _context.MatchTrackings.Any(mt => mt.IdMatch == matchId);
    }
}
