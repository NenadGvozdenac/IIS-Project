using System;
using System.Collections.Generic;

namespace match_service.src.Matches.Core.Domain.Entities;

public partial class Match
{
    public int IdMatch { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly CreatedAt { get; set; }

    public string Type { get; set; } = null!;

    public string State { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Hall { get; set; } = null!;

    public bool IsInOurHall { get; set; }

    public bool TransportationRequired { get; set; }

    public bool AccommodationRequired { get; set; }

    public int? IdCompetition { get; set; }

    public int IdSeason { get; set; }

    public int IdTeam { get; set; }

    public virtual Competition? IdCompetitionNavigation { get; set; }

    public virtual Season IdSeasonNavigation { get; set; } = null!;

    public virtual Team IdTeamNavigation { get; set; } = null!;

    public virtual MatchTracking? MatchTracking { get; set; }
}
