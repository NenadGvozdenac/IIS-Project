using System;
using System.Collections.Generic;

namespace ticket_service.src.Tickets.Core.Domain.Entities.Relational;

public partial class MatchSummaryReportView
{
    public int? MatchId { get; set; }

    public string? MatchName { get; set; }

    public DateTime? MatchDate { get; set; }

    public string? MatchType { get; set; }

    public string? City { get; set; }

    public string? Hall { get; set; }

    public string? SeasonName { get; set; }

    public string? CompetitionName { get; set; }

    public string? TeamName { get; set; }

    public int? TotalTicketsSold { get; set; }

    public decimal? TotalRevenue { get; set; }

    public int? VipZoneTickets { get; set; }

    public decimal? VipZoneRevenue { get; set; }

    public int? RegularZoneTickets { get; set; }

    public decimal? RegularZoneRevenue { get; set; }

    public decimal? AverageTicketPrice { get; set; }

    public decimal? StadiumFillPercentage { get; set; }

    public string? HighestSellingZone { get; set; }

    public string? LowestSellingZone { get; set; }

    public string? TrackingStatus { get; set; }

    public int? OurPoints { get; set; }

    public int? OpponentPoints { get; set; }
}
