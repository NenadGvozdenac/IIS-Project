using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Application.Utilities
{
    public static class PeriodTimeCalculator
    {
        /// <summary>
        /// Calculates the remaining time in the current period in milliseconds
        /// </summary>
        /// <param name="matchTracking">Match tracking entity with current state</param>
        /// <returns>Remaining time in milliseconds (null if cannot be calculated)</returns>
        public static int? CalculateRemainingPeriodTime(MatchTracking matchTracking)
        {
            if (matchTracking?.PeriodDuration == null)
                return null;

            int currentElapsed;

            if (matchTracking.PeriodStatus == "active")
            {
                // Period is active: calculate elapsed time
                if (matchTracking.PeriodStartTime == null)
                    return null;

                var periodElapsed = (int)(DateTime.UtcNow - matchTracking.PeriodStartTime.Value).TotalMilliseconds;
                var totalPauses = matchTracking.TotalPauseTimeInPeriod ?? 0;
                currentElapsed = Math.Max(0, periodElapsed - totalPauses);
            }
            else if (matchTracking.PeriodStatus == "upcoming")
            {
                // Period hasn't started yet: no elapsed time
                currentElapsed = 0;
            }
            else
            {
                // Period is paused/finished: use stored elapsed time
                currentElapsed = matchTracking.ElapsedPeriodTime ?? 0;
            }

            // Calculate remaining time
            var remaining = Math.Max(0, matchTracking.PeriodDuration.Value - currentElapsed);
            return remaining;
        }

        /// <summary>
        /// Calculates the current elapsed time in the period (for tracking purposes)
        /// </summary>
        /// <param name="matchTracking">Match tracking entity with current state</param>
        /// <returns>Elapsed time in milliseconds (null if cannot be calculated)</returns>
        public static int? CalculateElapsedPeriodTime(MatchTracking matchTracking)
        {
            if (matchTracking?.PeriodStatus == "active")
            {
                if (matchTracking.PeriodStartTime == null)
                    return null;

                var periodElapsed = (int)(DateTime.UtcNow - matchTracking.PeriodStartTime.Value).TotalMilliseconds;
                var totalPauses = matchTracking.TotalPauseTimeInPeriod ?? 0;
                return Math.Max(0, periodElapsed - totalPauses);
            }
            else if (matchTracking?.PeriodStatus == "upcoming")
            {
                // Period hasn't started yet: no elapsed time
                return 0;
            }
            else
            {
                // Period is paused/finished: use stored elapsed time
                return matchTracking?.ElapsedPeriodTime ?? 0;
            }
        }
    }
}