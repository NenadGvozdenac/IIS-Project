using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Infrastructure;

public partial class TravelDbContext : DbContext
{
    public TravelDbContext()
    {
    }

    public TravelDbContext(DbContextOptions<TravelDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccommodationOffer> AccommodationOffers { get; set; }

    public virtual DbSet<AccommodationRequest> AccommodationRequests { get; set; }

    public virtual DbSet<Agency> Agencies { get; set; }

    public virtual DbSet<Competition> Competitions { get; set; }

    public virtual DbSet<Management> Managements { get; set; }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<Nationality> Nationalities { get; set; }

    public virtual DbSet<Offer> Offers { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Season> Seasons { get; set; }

    public virtual DbSet<SentRequest> SentRequests { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TeamMember> TeamMembers { get; set; }

    public virtual DbSet<TransportationOffer> TransportationOffers { get; set; }

    public virtual DbSet<TransportationRequest> TransportationRequests { get; set; }

    public virtual DbSet<TravelInformation> TravelInformations { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Visa> Visas { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=sportsdb;Username=postgres;Password=postgres;Port=5432");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccommodationOffer>(entity =>
        {
            entity.HasKey(e => new { e.IdOffer, e.IdAgency, e.IdRequest }).HasName("accommodation_offer_pkey");

            entity.ToTable("accommodation_offer");

            entity.Property(e => e.IdOffer).HasColumnName("id_offer");
            entity.Property(e => e.IdAgency).HasColumnName("id_agency");
            entity.Property(e => e.IdRequest).HasColumnName("id_request");
            entity.Property(e => e.AccommodationType)
                .HasMaxLength(20)
                .HasColumnName("accommodation_type");
            entity.Property(e => e.Breakfast).HasColumnName("breakfast");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.DoubleRoom).HasColumnName("double_room");
            entity.Property(e => e.FitnessCenter).HasColumnName("fitness_center");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Pool).HasColumnName("pool");
            entity.Property(e => e.QuadrupleRoom).HasColumnName("quadruple_room");
            entity.Property(e => e.Spa).HasColumnName("spa");
            entity.Property(e => e.TripleRoom).HasColumnName("triple_room");
            entity.Property(e => e.Wifi).HasColumnName("wifi");

            entity.HasOne(d => d.Id).WithOne(p => p.AccommodationOffer)
                .HasForeignKey<AccommodationOffer>(d => new { d.IdOffer, d.IdAgency, d.IdRequest })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_acc_offer_offer");
        });

        modelBuilder.Entity<AccommodationRequest>(entity =>
        {
            entity.HasKey(e => e.IdRequest).HasName("accommodation_request_pkey");

            entity.ToTable("accommodation_request");

            entity.Property(e => e.IdRequest)
                .ValueGeneratedOnAdd()
                .HasColumnName("id_request");
            entity.Property(e => e.AccommodationType)
                .HasMaxLength(20)
                .HasColumnName("accommodation_type");
            entity.Property(e => e.CheckInDate).HasColumnName("check_in_date");
            entity.Property(e => e.CheckOutDate).HasColumnName("check_out_date");
            entity.Property(e => e.NumberOfGuests).HasColumnName("number_of_guests");
            entity.Property(e => e.NumberOfRooms).HasColumnName("number_of_rooms");

            entity.HasOne(d => d.IdRequestNavigation).WithOne(p => p.AccommodationRequest)
                .HasForeignKey<AccommodationRequest>(d => d.IdRequest)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_acc_request_request");
        });

        modelBuilder.Entity<Agency>(entity =>
        {
            entity.HasKey(e => e.IdAgency).HasName("agency_pkey");

            entity.ToTable("agency");

            entity.Property(e => e.IdAgency).HasColumnName("id_agency");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
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

        modelBuilder.Entity<Management>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("management_pkey");

            entity.ToTable("management");

            entity.Property(e => e.MemberId).HasColumnName("member_id");
            entity.Property(e => e.MemberName)
                .HasMaxLength(255)
                .HasColumnName("member_name");
            entity.Property(e => e.MemberRole)
                .HasMaxLength(20)
                .HasColumnName("member_role");
            entity.Property(e => e.MemberSurname)
                .HasMaxLength(255)
                .HasColumnName("member_surname");
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.IdMatch).HasName("match_pkey");

            entity.ToTable("match");

            entity.HasIndex(e => e.ScheduledAt, "idx_match_scheduled_at");

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

        modelBuilder.Entity<Nationality>(entity =>
        {
            entity.HasKey(e => e.IdNationality).HasName("nationality_pkey");

            entity.ToTable("nationality");

            entity.Property(e => e.IdNationality).HasColumnName("id_nationality");
            entity.Property(e => e.State)
                .HasMaxLength(255)
                .HasColumnName("state");
        });

        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasKey(e => new { e.IdOffer, e.IdAgency, e.IdRequest }).HasName("offer_pkey");

            entity.ToTable("offer");

            entity.Property(e => e.IdOffer).HasColumnName("id_offer");
            entity.Property(e => e.IdAgency).HasColumnName("id_agency");
            entity.Property(e => e.IdRequest).HasColumnName("id_request");
            entity.Property(e => e.IdMatch).HasColumnName("id_match");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.UserIdUser).HasColumnName("user_id_user");

            entity.HasOne(d => d.IdMatchNavigation).WithMany(p => p.Offers)
                .HasForeignKey(d => d.IdMatch)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_offer_match");

            entity.HasOne(d => d.UserIdUserNavigation).WithMany(p => p.Offers)
                .HasForeignKey(d => d.UserIdUser)
                .HasConstraintName("fk_offer_user");

            entity.HasOne(d => d.Id).WithMany(p => p.Offers)
                .HasForeignKey(d => new { d.IdAgency, d.IdRequest })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_offer_sent_request");
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

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.IdRequest).HasName("request_pkey");

            entity.ToTable("request");

            entity.Property(e => e.IdRequest).HasColumnName("id_request");
            entity.Property(e => e.Budget).HasColumnName("budget");
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city");
            entity.Property(e => e.Hall)
                .HasMaxLength(255)
                .HasColumnName("hall");
            entity.Property(e => e.IdMatch).HasColumnName("id_match");
            entity.Property(e => e.State)
                .HasMaxLength(255)
                .HasColumnName("state");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.IdMatchNavigation).WithMany(p => p.Requests)
                .HasForeignKey(d => d.IdMatch)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_request_match");

            entity.HasMany(d => d.IdManagementMembers).WithMany(p => p.IdRequests)
                .UsingEntity<Dictionary<string, object>>(
                    "ManagementMemberRequest",
                    r => r.HasOne<Management>().WithMany()
                        .HasForeignKey("IdManagementMember")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_mgmt_mem_req_management"),
                    l => l.HasOne<Request>().WithMany()
                        .HasForeignKey("IdRequest")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_mgmt_mem_req_request"),
                    j =>
                    {
                        j.HasKey("IdRequest", "IdManagementMember").HasName("management_member_request_pkey");
                        j.ToTable("management_member_request");
                        j.IndexerProperty<int>("IdRequest").HasColumnName("id_request");
                        j.IndexerProperty<int>("IdManagementMember").HasColumnName("id_management_member");
                    });
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

        modelBuilder.Entity<SentRequest>(entity =>
        {
            entity.HasKey(e => new { e.IdAgency, e.IdRequest }).HasName("sent_request_pkey");

            entity.ToTable("sent_request");

            entity.Property(e => e.IdAgency).HasColumnName("id_agency");
            entity.Property(e => e.IdRequest).HasColumnName("id_request");

            entity.HasOne(d => d.IdAgencyNavigation).WithMany(p => p.SentRequests)
                .HasForeignKey(d => d.IdAgency)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sent_request_agency");

            entity.HasOne(d => d.IdRequestNavigation).WithMany(p => p.SentRequests)
                .HasForeignKey(d => d.IdRequest)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sent_request_request");
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

            entity.HasMany(d => d.IdRequests).WithMany(p => p.Ids)
                .UsingEntity<Dictionary<string, object>>(
                    "TeamMemberRequest",
                    r => r.HasOne<Request>().WithMany()
                        .HasForeignKey("IdRequest")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_team_mem_req_request"),
                    l => l.HasOne<TeamMember>().WithMany()
                        .HasForeignKey("IdTeam", "IdPlayer")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_team_mem_req_team_member"),
                    j =>
                    {
                        j.HasKey("IdTeam", "IdPlayer", "IdRequest").HasName("team_member_request_pkey");
                        j.ToTable("team_member_request");
                        j.IndexerProperty<int>("IdTeam").HasColumnName("id_team");
                        j.IndexerProperty<int>("IdPlayer").HasColumnName("id_player");
                        j.IndexerProperty<int>("IdRequest").HasColumnName("id_request");
                    });
        });

        modelBuilder.Entity<TransportationOffer>(entity =>
        {
            entity.HasKey(e => new { e.IdOffer, e.IdAgency, e.IdRequest }).HasName("transportation_offer_pkey");

            entity.ToTable("transportation_offer");

            entity.Property(e => e.IdOffer).HasColumnName("id_offer");
            entity.Property(e => e.IdAgency).HasColumnName("id_agency");
            entity.Property(e => e.IdRequest).HasColumnName("id_request");
            entity.Property(e => e.AirConditioning).HasColumnName("air_conditioning");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .HasColumnName("company_name");
            entity.Property(e => e.EquipmentSpace).HasColumnName("equipment_space");
            entity.Property(e => e.Restroom).HasColumnName("restroom");
            entity.Property(e => e.Tv).HasColumnName("tv");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.Wifi).HasColumnName("wifi");

            entity.HasOne(d => d.Id).WithOne(p => p.TransportationOffer)
                .HasForeignKey<TransportationOffer>(d => new { d.IdOffer, d.IdAgency, d.IdRequest })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_trans_offer_offer");
        });

        modelBuilder.Entity<TransportationRequest>(entity =>
        {
            entity.HasKey(e => e.IdRequest).HasName("transportation_request_pkey");

            entity.ToTable("transportation_request");

            entity.Property(e => e.IdRequest)
                .ValueGeneratedNever()
                .HasColumnName("id_request");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.NumberOfPassengers).HasColumnName("number_of_passengers");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.VehicleType)
                .HasMaxLength(20)
                .HasColumnName("vehicle_type");

            entity.HasOne(d => d.IdRequestNavigation).WithOne(p => p.TransportationRequest)
                .HasForeignKey<TransportationRequest>(d => d.IdRequest)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_trans_request_request");
        });

        modelBuilder.Entity<TravelInformation>(entity =>
        {
            entity.HasKey(e => e.IdTravelInformation).HasName("travel_information_pkey");

            entity.ToTable("travel_information");

            entity.HasIndex(e => e.IdManagementMember, "travel_information_id_management_member_key").IsUnique();

            entity.HasIndex(e => new { e.IdTeam, e.IdPlayer }, "travel_information_id_team_id_player_key").IsUnique();

            entity.Property(e => e.IdTravelInformation).HasColumnName("id_travel_information");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.IdManagementMember).HasColumnName("id_management_member");
            entity.Property(e => e.IdPlayer).HasColumnName("id_player");
            entity.Property(e => e.IdTeam).HasColumnName("id_team");
            entity.Property(e => e.PassportExpirationDate).HasColumnName("passport_expiration_date");
            entity.Property(e => e.PassportNumber)
                .HasMaxLength(30)
                .HasColumnName("passport_number");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");

            entity.HasOne(d => d.IdManagementMemberNavigation).WithOne(p => p.TravelInformation)
                .HasForeignKey<TravelInformation>(d => d.IdManagementMember)
                .HasConstraintName("fk_travel_info_management");

            entity.HasOne(d => d.Id).WithOne(p => p.TravelInformation)
                .HasForeignKey<TravelInformation>(d => new { d.IdTeam, d.IdPlayer })
                .HasConstraintName("fk_travel_info_team_member");
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.IdTrip).HasName("trip_pkey");

            entity.ToTable("trip");

            entity.HasIndex(e => new { e.IdAccommodationOffer, e.IdAccommodationAgency, e.IdAccommodationRequest }, "trip_id_accommodation_offer_id_accommodation_agency_id_acco_key").IsUnique();

            entity.HasIndex(e => new { e.IdTransportationOffer, e.IdTransportationAgency, e.IdTransportationRequest }, "trip_id_transportation_offer_id_transportation_agency_id_tr_key").IsUnique();

            entity.HasIndex(e => e.MatchIdMatch, "trip_match_id_match_key").IsUnique();

            entity.Property(e => e.IdTrip).HasColumnName("id_trip");
            entity.Property(e => e.IdAccommodationAgency).HasColumnName("id_accommodation_agency");
            entity.Property(e => e.IdAccommodationOffer).HasColumnName("id_accommodation_offer");
            entity.Property(e => e.IdAccommodationRequest).HasColumnName("id_accommodation_request");
            entity.Property(e => e.IdTransportationAgency).HasColumnName("id_transportation_agency");
            entity.Property(e => e.IdTransportationOffer).HasColumnName("id_transportation_offer");
            entity.Property(e => e.IdTransportationRequest).HasColumnName("id_transportation_request");
            entity.Property(e => e.MatchIdMatch).HasColumnName("match_id_match");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .HasColumnName("notes");

            entity.HasOne(d => d.MatchIdMatchNavigation).WithOne(p => p.Trip)
                .HasForeignKey<Trip>(d => d.MatchIdMatch)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_trip_match");

            entity.HasOne(d => d.IdAccommodation).WithOne(p => p.Trip)
                .HasForeignKey<Trip>(d => new { d.IdAccommodationOffer, d.IdAccommodationAgency, d.IdAccommodationRequest })
                .HasConstraintName("fk_trip_accommodation_offer");

            entity.HasOne(d => d.IdTransportation).WithOne(p => p.Trip)
                .HasForeignKey<Trip>(d => new { d.IdTransportationOffer, d.IdTransportationAgency, d.IdTransportationRequest })
                .HasConstraintName("fk_trip_transportation_offer");
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

        modelBuilder.Entity<Visa>(entity =>
        {
            entity.HasKey(e => e.VisaNumber).HasName("visa_pkey");

            entity.ToTable("visa");

            entity.Property(e => e.VisaNumber)
                .HasMaxLength(20)
                .HasColumnName("visa_number");
            entity.Property(e => e.CreationDate).HasColumnName("creation_date");
            entity.Property(e => e.ExpirationDate).HasColumnName("expiration_date");
            entity.Property(e => e.IdTravelInformation).HasColumnName("id_travel_information");
            entity.Property(e => e.State)
                .HasMaxLength(255)
                .HasColumnName("state");

            entity.HasOne(d => d.IdTravelInformationNavigation).WithMany(p => p.Visas)
                .HasForeignKey(d => d.IdTravelInformation)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_visa_travel_information");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
