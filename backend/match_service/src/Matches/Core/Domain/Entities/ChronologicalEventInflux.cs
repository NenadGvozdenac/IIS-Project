using InfluxDB.Client.Core;
using System;

namespace match_service.src.Matches.Core.Domain.Entities;

[Measurement("basketball_events")]
public class ChronologicalEventInflux
{
    // Primary timestamp field for time-series data
    [Column(IsTimestamp = true)]
    public DateTime Timestamp { get; set; }

    // Tags (indexed fields for fast queries) - immutable dimensions
    [Column("match_id", IsTag = true)]
    public string MatchId { get; set; } = string.Empty;

    [Column("event_category", IsTag = true)]
    public string EventCategory { get; set; } = string.Empty; // "personal", "team", "general"

    [Column("event_type", IsTag = true)]
    public string EventType { get; set; } = string.Empty; // "foul", "shot", "timeout", etc.

    [Column("period", IsTag = true)]
    public string Period { get; set; } = string.Empty;

    [Column("team_id", IsTag = true)]
    public string? TeamId { get; set; }

    [Column("player_id", IsTag = true)]
    public string? PlayerId { get; set; }

    // Fields (data values) - measured metrics
    [Column("event_id")]
    public int EventId { get; set; }

    [Column("player_name")]
    public string? PlayerName { get; set; }

    [Column("period_time")]
    public int? PeriodTime { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("creation_time")]
    public DateTime CreationTime { get; set; }

    // Additional metadata for basketball-specific analytics
    [Column("our_points")]
    public int? OurPoints { get; set; }

    [Column("opponent_points")]
    public int? OpponentPoints { get; set; }

    [Column("point_difference")]
    public int? PointDifference { get; set; }

    // Constructor
    public ChronologicalEventInflux()
    {
        Timestamp = DateTime.UtcNow;
        CreationTime = DateTime.UtcNow;
    }

    // Factory methods for creating events from existing entities
    public static ChronologicalEventInflux FromPersonalEvent(PersonalEvent personalEvent, string? playerName = null, int? ourPoints = null, int? opponentPoints = null)
    {
        return new ChronologicalEventInflux
        {
            Timestamp = personalEvent.CreationTime,
            EventId = personalEvent.IdEvent,
            MatchId = personalEvent.IdMatch.ToString(),
            EventCategory = "personal",
            EventType = personalEvent.Type ?? "unknown",
            Period = personalEvent.Period ?? "unknown",
            PeriodTime = personalEvent.PeriodTime,
            TeamId = personalEvent.IdTeam.ToString(),
            PlayerId = personalEvent.IdPlayer.ToString(),
            PlayerName = playerName,
            Notes = personalEvent.Notes,
            CreationTime = personalEvent.CreationTime,
            OurPoints = ourPoints,
            OpponentPoints = opponentPoints,
            PointDifference = ourPoints.HasValue && opponentPoints.HasValue ? ourPoints.Value - opponentPoints.Value : null
        };
    }

    public static ChronologicalEventInflux FromTeamEvent(TeamEvent teamEvent, int? ourPoints = null, int? opponentPoints = null)
    {
        return new ChronologicalEventInflux
        {
            Timestamp = teamEvent.CreationTime,
            EventId = teamEvent.IdEvent,
            MatchId = teamEvent.IdMatch.ToString(),
            EventCategory = "team",
            EventType = teamEvent.Type ?? "unknown",
            Period = teamEvent.Period ?? "unknown",
            PeriodTime = teamEvent.PeriodTime,
            TeamId = teamEvent.IdTeam.ToString(),
            PlayerId = null,
            Notes = teamEvent.Notes,
            CreationTime = teamEvent.CreationTime,
            OurPoints = ourPoints,
            OpponentPoints = opponentPoints,
            PointDifference = ourPoints.HasValue && opponentPoints.HasValue ? ourPoints.Value - opponentPoints.Value : null
        };
    }

    public static ChronologicalEventInflux FromGeneralEvent(GeneralEvent generalEvent, int? ourPoints = null, int? opponentPoints = null)
    {
        return new ChronologicalEventInflux
        {
            Timestamp = generalEvent.CreationTime,
            EventId = generalEvent.IdEvent,
            MatchId = generalEvent.IdMatch.ToString(),
            EventCategory = "general",
            EventType = generalEvent.Type ?? "unknown",
            Period = generalEvent.Period ?? "unknown",
            PeriodTime = generalEvent.PeriodTime,
            TeamId = null,
            PlayerId = null,
            Notes = generalEvent.Notes,
            CreationTime = generalEvent.CreationTime,
            OurPoints = ourPoints,
            OpponentPoints = opponentPoints,
            PointDifference = ourPoints.HasValue && opponentPoints.HasValue ? ourPoints.Value - opponentPoints.Value : null
        };
    }
}