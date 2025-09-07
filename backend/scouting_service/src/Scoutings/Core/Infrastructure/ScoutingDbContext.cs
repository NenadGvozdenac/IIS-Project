using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Infrastructure;

public partial class ScoutingDbContext : DbContext
{
    public ScoutingDbContext()
    {
    }

    public ScoutingDbContext(DbContextOptions<ScoutingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Metric> Metrics { get; set; }

    public virtual DbSet<MetricType> MetricTypes { get; set; }

    public virtual DbSet<Nationality> Nationalities { get; set; }

    public virtual DbSet<PhysicalMetric> PhysicalMetrics { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Season> Seasons { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<SessionMetric> SessionMetrics { get; set; }

    public virtual DbSet<SessionStatus> SessionStatuses { get; set; }

    public virtual DbSet<SessionType> SessionTypes { get; set; }

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
        modelBuilder.Entity<Metric>(entity =>
        {
            entity.HasKey(e => e.IdMetrics).HasName("metrics_pkey");

            entity.ToTable("metrics");

            entity.Property(e => e.IdMetrics).HasColumnName("id_metrics");
            entity.Property(e => e.IdMetricType).HasColumnName("id_metric_type");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.IsPermanent).HasColumnName("is_permanent");
            entity.Property(e => e.MetricWeight).HasColumnName("metric_weight");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.HasOne(d => d.IdMetricTypeNavigation).WithMany(p => p.Metrics)
                .HasForeignKey(d => d.IdMetricType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_metrics_metric_type");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Metrics)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_metrics_user");
        });

        modelBuilder.Entity<MetricType>(entity =>
        {
            entity.HasKey(e => e.IdType).HasName("metric_type_pkey");

            entity.ToTable("metric_type");

            entity.Property(e => e.IdType).HasColumnName("id_type");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");
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
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.IdSession).HasName("session_pkey");

            entity.ToTable("session");

            entity.Property(e => e.IdSession).HasColumnName("id_session");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.IdPlayer).HasColumnName("id_player");
            entity.Property(e => e.IdSessionStatus).HasColumnName("id_session_status");
            entity.Property(e => e.IdSessionType).HasColumnName("id_session_type");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.StartTime).HasColumnName("start_time");

            entity.HasOne(d => d.IdPlayerNavigation).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.IdPlayer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_session_player");

            entity.HasOne(d => d.IdSessionStatusNavigation).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.IdSessionStatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_session_session_status");

            entity.HasOne(d => d.IdSessionTypeNavigation).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.IdSessionType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_session_session_type");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_session_user");
        });

        modelBuilder.Entity<SessionMetric>(entity =>
        {
            entity.HasKey(e => new { e.IdMetrics, e.IdSession }).HasName("session_metrics_pkey");

            entity.ToTable("session_metrics");

            entity.Property(e => e.IdMetrics).HasColumnName("id_metrics");
            entity.Property(e => e.IdSession).HasColumnName("id_session");
            entity.Property(e => e.Value)
                .HasMaxLength(255)
                .HasColumnName("value");

            entity.HasOne(d => d.IdMetricsNavigation).WithMany(p => p.SessionMetrics)
                .HasForeignKey(d => d.IdMetrics)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_session_metrics_metrics");

            entity.HasOne(d => d.IdSessionNavigation).WithMany(p => p.SessionMetrics)
                .HasForeignKey(d => d.IdSession)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_session_metrics_session");
        });

        modelBuilder.Entity<SessionStatus>(entity =>
        {
            entity.HasKey(e => e.IdStatus).HasName("session_status_pkey");

            entity.ToTable("session_status");

            entity.Property(e => e.IdStatus).HasColumnName("id_status");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
        });

        modelBuilder.Entity<SessionType>(entity =>
        {
            entity.HasKey(e => e.IdType).HasName("session_type_pkey");

            entity.ToTable("session_type");

            entity.Property(e => e.IdType).HasColumnName("id_type");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");
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
