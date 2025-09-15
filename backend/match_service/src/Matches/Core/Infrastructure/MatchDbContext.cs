using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.Core.Infrastructure;

public partial class MatchDbContext : DbContext
{
    public MatchDbContext()
    {
    }

    public MatchDbContext(DbContextOptions<MatchDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AutomaticRecommendation> AutomaticRecommendations { get; set; }

    public virtual DbSet<Competition> Competitions { get; set; }

    public virtual DbSet<GeneralEvent> GeneralEvents { get; set; }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<MatchTracking> MatchTrackings { get; set; }

    public virtual DbSet<Nationality> Nationalities { get; set; }

    public virtual DbSet<PersonalEvent> PersonalEvents { get; set; }

    public virtual DbSet<PhysicalMetric> PhysicalMetrics { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Season> Seasons { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TeamEvent> TeamEvents { get; set; }

    public virtual DbSet<TeamMember> TeamMembers { get; set; }

    public virtual DbSet<TeamMemberMatch> TeamMemberMatches { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=sportsdb;Username=postgres;Password=postgres;Port=5432");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AutomaticRecommendation>(entity =>
        {
            entity.HasKey(e => e.IdMatch).HasName("automatic_recommendations_pkey");

            entity.ToTable("automatic_recommendations");

            entity.Property(e => e.IdMatch)
                .ValueGeneratedNever()
                .HasColumnName("id_match");
            entity.Property(e => e.CreationTime).HasColumnName("creation_time");
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .HasColumnName("priority");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.IdMatchNavigation).WithOne(p => p.AutomaticRecommendation)
                .HasForeignKey<AutomaticRecommendation>(d => d.IdMatch)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_auto_rec_match_tracking");
        });

        modelBuilder.Entity<Competition>(entity =>
        {
            entity.HasKey(e => e.IdCompetition).HasName("competition_pkey");

            entity.ToTable("competition");

            entity.Property(e => e.IdCompetition).HasColumnName("id_competition");
            entity.Property(e => e.EndedAt).HasColumnName("ended_at");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.NumberOfMatches).HasColumnName("number_of_matches");
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
        });

        modelBuilder.Entity<GeneralEvent>(entity =>
        {
            entity.HasKey(e => e.IdEvent).HasName("general_event_pkey");

            entity.ToTable("general_event");

            entity.HasIndex(e => e.IdMatch, "idx_general_event_id_match");

            entity.Property(e => e.IdEvent).HasColumnName("id_event");
            entity.Property(e => e.CreationTime).HasColumnName("creation_time");
            entity.Property(e => e.IdMatch).HasColumnName("id_match");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .HasColumnName("notes");
            entity.Property(e => e.Period)
                .HasMaxLength(20)
                .HasColumnName("period");
            entity.Property(e => e.PeriodTime).HasColumnName("period_time");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.IdMatchNavigation).WithMany(p => p.GeneralEvents)
                .HasForeignKey(d => d.IdMatch)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_general_event_match_tracking");
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.IdMatch).HasName("match_pkey");

            entity.ToTable("match");

            entity.Property(e => e.IdMatch).HasColumnName("id_match");
            entity.Property(e => e.AccommodationRequired).HasColumnName("accommodation_required");
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city");
            entity.Property(e => e.Hall)
                .HasMaxLength(255)
                .HasColumnName("hall");
            entity.Property(e => e.IdCompetition).HasColumnName("id_competition");
            entity.Property(e => e.IdSeason).HasColumnName("id_season");
            entity.Property(e => e.IdTeam).HasColumnName("id_team");
            entity.Property(e => e.IsInOurHall).HasColumnName("is_in_our_hall");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.ScheduledAt).HasColumnName("scheduled_at");
            entity.Property(e => e.State)
                .HasMaxLength(255)
                .HasColumnName("state");
            entity.Property(e => e.TicketsForSale).HasColumnName("tickets_for_sale");
            entity.Property(e => e.TicketsWentOnSale).HasColumnName("tickets_went_on_sale");
            entity.Property(e => e.TransportationRequired).HasColumnName("transportation_required");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.IdCompetitionNavigation).WithMany(p => p.Matches)
                .HasForeignKey(d => d.IdCompetition)
                .HasConstraintName("fk_match_competition");

            entity.HasOne(d => d.IdSeasonNavigation).WithMany(p => p.Matches)
                .HasForeignKey(d => d.IdSeason)
                .HasConstraintName("fk_match_season");

            entity.HasOne(d => d.IdTeamNavigation).WithMany(p => p.Matches)
                .HasForeignKey(d => d.IdTeam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_match_team");
        });

        modelBuilder.Entity<MatchTracking>(entity =>
        {
            entity.HasKey(e => e.IdMatch).HasName("match_tracking_pkey");

            entity.ToTable("match_tracking");

            entity.Property(e => e.IdMatch)
                .ValueGeneratedNever()
                .HasColumnName("id_match");
            entity.Property(e => e.CurrentPeriod)
                .HasMaxLength(20)
                .HasColumnName("current_period");
            entity.Property(e => e.ElapsedPeriodTime).HasColumnName("elapsed_period_time");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.LastPauseStartTime).HasColumnName("last_pause_start_time");
            entity.Property(e => e.LastUpdateTime).HasColumnName("last_update_time");
            entity.Property(e => e.OpponentPoints).HasColumnName("opponent_points");
            entity.Property(e => e.OurPoints).HasColumnName("our_points");
            entity.Property(e => e.PeriodDuration).HasColumnName("period_duration");
            entity.Property(e => e.PeriodStartTime).HasColumnName("period_start_time");
            entity.Property(e => e.PeriodStatus)
                .HasMaxLength(20)
                .HasColumnName("period_status");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.TotalPauseTimeInPeriod).HasColumnName("total_pause_time_in_period");
            entity.Property(e => e.TrackingStatus)
                .HasMaxLength(20)
                .HasColumnName("tracking_status");

            entity.HasOne(d => d.IdMatchNavigation).WithOne(p => p.MatchTracking)
                .HasForeignKey<MatchTracking>(d => d.IdMatch)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_match_tracking_match");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.MatchTrackings)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("fk_match_tracking_user");
        });

        modelBuilder.Entity<Nationality>(entity =>
        {
            entity.HasKey(e => e.IdNationality).HasName("nationality_pkey");

            entity.ToTable("nationality");

            entity.Property(e => e.IdNationality).HasColumnName("id_nationality");
            entity.Property(e => e.State)
                .HasMaxLength(255)
                .HasColumnName("state");
        });

        modelBuilder.Entity<PersonalEvent>(entity =>
        {
            entity.HasKey(e => e.IdEvent).HasName("personal_event_pkey");

            entity.ToTable("personal_event");

            entity.HasIndex(e => e.IdMatch, "idx_personal_event_id_match");

            entity.HasIndex(e => e.IdPlayer, "idx_personal_event_id_player");

            entity.HasIndex(e => e.IdTeam, "idx_personal_event_id_team");

            entity.HasIndex(e => new { e.IdTeam, e.IdPlayer }, "idx_personal_event_team_player");

            entity.HasIndex(e => e.Type, "idx_personal_event_type");

            entity.Property(e => e.IdEvent).HasColumnName("id_event");
            entity.Property(e => e.CreationTime).HasColumnName("creation_time");
            entity.Property(e => e.IdMatch).HasColumnName("id_match");
            entity.Property(e => e.IdPlayer).HasColumnName("id_player");
            entity.Property(e => e.IdTeam).HasColumnName("id_team");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .HasColumnName("notes");
            entity.Property(e => e.Period)
                .HasMaxLength(20)
                .HasColumnName("period");
            entity.Property(e => e.PeriodTime).HasColumnName("period_time");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.IdMatchNavigation).WithMany(p => p.PersonalEvents)
                .HasForeignKey(d => d.IdMatch)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_personal_event_match_tracking");

            entity.HasOne(d => d.Id).WithMany(p => p.PersonalEvents)
                .HasForeignKey(d => new { d.IdTeam, d.IdPlayer })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_personal_event_team_member");
        });

        modelBuilder.Entity<PhysicalMetric>(entity =>
        {
            entity.HasKey(e => e.IdPhysicalMetrics).HasName("physical_metrics_pkey");

            entity.ToTable("physical_metrics");

            entity.Property(e => e.IdPhysicalMetrics).HasColumnName("id_physical_metrics");
            entity.Property(e => e.BenchPressWeight).HasColumnName("bench_press_weight");
            entity.Property(e => e.DateOfMeasurement).HasColumnName("date_of_measurement");
            entity.Property(e => e.FatPercentage).HasColumnName("fat_percentage");
            entity.Property(e => e.Height).HasColumnName("height");
            entity.Property(e => e.IdPlayer).HasColumnName("id_player");
            entity.Property(e => e.SprintSpeed).HasColumnName("sprint_speed");
            entity.Property(e => e.SquatWeight).HasColumnName("squat_weight");
            entity.Property(e => e.VerticalJump).HasColumnName("vertical_jump");
            entity.Property(e => e.Weight).HasColumnName("weight");
            entity.Property(e => e.Wingspan).HasColumnName("wingspan");

            entity.HasOne(d => d.IdPlayerNavigation).WithMany(p => p.PhysicalMetrics)
                .HasForeignKey(d => d.IdPlayer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_physical_metrics_player");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.IdPlayer).HasName("player_pkey");

            entity.ToTable("player");

            entity.Property(e => e.IdPlayer).HasColumnName("id_player");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.Height).HasColumnName("height");
            entity.Property(e => e.IdNationality).HasColumnName("id_nationality");
            entity.Property(e => e.IdPosition).HasColumnName("id_position");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");
            entity.Property(e => e.Weight).HasColumnName("weight");

            entity.HasOne(d => d.IdNationalityNavigation).WithMany(p => p.Players)
                .HasForeignKey(d => d.IdNationality)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_player_nationality");

            entity.HasOne(d => d.IdPositionNavigation).WithMany(p => p.Players)
                .HasForeignKey(d => d.IdPosition)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_player_position");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.IdPosition).HasName("position_pkey");

            entity.ToTable("position");

            entity.Property(e => e.IdPosition).HasColumnName("id_position");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Season>(entity =>
        {
            entity.HasKey(e => e.IdSeason).HasName("season_pkey");

            entity.ToTable("season");

            entity.Property(e => e.IdSeason).HasColumnName("id_season");
            entity.Property(e => e.EndedAt).HasColumnName("ended_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.TicketsForSale).HasColumnName("tickets_for_sale");
            entity.Property(e => e.TicketsWentOnSale).HasColumnName("tickets_went_on_sale");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.IdTeam).HasName("team_pkey");

            entity.ToTable("team");

            entity.Property(e => e.IdTeam).HasColumnName("id_team");
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city");
            entity.Property(e => e.Coach)
                .HasMaxLength(255)
                .HasColumnName("coach");
            entity.Property(e => e.FoundedDate).HasColumnName("founded_date");
            entity.Property(e => e.Hall)
                .HasMaxLength(255)
                .HasColumnName("hall");
            entity.Property(e => e.KeyStrengths)
                .HasMaxLength(255)
                .HasColumnName("key_strengths");
            entity.Property(e => e.KeyWeaknesses)
                .HasMaxLength(255)
                .HasColumnName("key_weaknesses");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PlayingStyle)
                .HasMaxLength(255)
                .HasColumnName("playing_style");
            entity.Property(e => e.State)
                .HasMaxLength(255)
                .HasColumnName("state");
        });

        modelBuilder.Entity<TeamEvent>(entity =>
        {
            entity.HasKey(e => e.IdEvent).HasName("team_event_pkey");

            entity.ToTable("team_event");

            entity.HasIndex(e => e.IdMatch, "idx_team_event_id_match");

            entity.Property(e => e.IdEvent).HasColumnName("id_event");
            entity.Property(e => e.CreationTime).HasColumnName("creation_time");
            entity.Property(e => e.IdMatch).HasColumnName("id_match");
            entity.Property(e => e.IdTeam).HasColumnName("id_team");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .HasColumnName("notes");
            entity.Property(e => e.Period)
                .HasMaxLength(20)
                .HasColumnName("period");
            entity.Property(e => e.PeriodTime).HasColumnName("period_time");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.IdMatchNavigation).WithMany(p => p.TeamEvents)
                .HasForeignKey(d => d.IdMatch)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_team_event_match_tracking");

            entity.HasOne(d => d.IdTeamNavigation).WithMany(p => p.TeamEvents)
                .HasForeignKey(d => d.IdTeam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_team_event_team");
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => new { e.IdTeam, e.IdPlayer }).HasName("team_member_pkey");

            entity.ToTable("team_member");

            entity.Property(e => e.IdTeam).HasColumnName("id_team");
            entity.Property(e => e.IdPlayer).HasColumnName("id_player");
            entity.Property(e => e.JerseyNumber).HasColumnName("jersey_number");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.IdPlayerNavigation).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.IdPlayer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_team_member_player");

            entity.HasOne(d => d.IdTeamNavigation).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.IdTeam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_team_member_team");
        });

        modelBuilder.Entity<TeamMemberMatch>(entity =>
        {
            entity.HasKey(e => new { e.IdMatch, e.IdTeam, e.IdPlayer }).HasName("team_member_match_pkey");

            entity.ToTable("team_member_match");

            entity.Property(e => e.IdMatch).HasColumnName("id_match");
            entity.Property(e => e.IdTeam).HasColumnName("id_team");
            entity.Property(e => e.IdPlayer).HasColumnName("id_player");
            entity.Property(e => e.InGame).HasColumnName("in_game");
            entity.Property(e => e.StartingLineup).HasColumnName("starting_lineup");

            entity.HasOne(d => d.IdMatchNavigation).WithMany(p => p.TeamMemberMatches)
                .HasForeignKey(d => d.IdMatch)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_team_mem_match_match_tracking");

            entity.HasOne(d => d.Id).WithMany(p => p.TeamMemberMatches)
                .HasForeignKey(d => new { d.IdTeam, d.IdPlayer })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_team_mem_match_team_member");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .HasColumnName("surname");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
