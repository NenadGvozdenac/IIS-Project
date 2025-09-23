using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Infrastructure;

public partial class TicketDbContext : DbContext
{
    public TicketDbContext()
    {
    }

    public TicketDbContext(DbContextOptions<TicketDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Competition> Competitions { get; set; }

    public virtual DbSet<CreditCard> CreditCards { get; set; }

    public virtual DbSet<IndividualTicket> IndividualTickets { get; set; }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<MatchSummaryReportView> MatchSummaryReportViews { get; set; }

    public virtual DbSet<MatchZoneSalesSummary> MatchZoneSalesSummaries { get; set; }

    public virtual DbSet<PurchaseOffer> PurchaseOffers { get; set; }

    public virtual DbSet<Season> Seasons { get; set; }

    public virtual DbSet<SeasonTicket> SeasonTickets { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TicketPriceParameter> TicketPriceParameters { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Zone> Zones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=sportsdb;Username=postgres;Password=postgres;Port=5432");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.IdCart).HasName("cart_pkey");

            entity.ToTable("cart");

            entity.HasIndex(e => e.Status, "idx_cart_status");

            entity.HasIndex(e => new { e.IdUser, e.Status }, "idx_cart_user_status");

            entity.Property(e => e.IdCart).HasColumnName("id_cart");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.IdCreditCard).HasColumnName("id_credit_card");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.IsCurrent).HasColumnName("is_current");
            entity.Property(e => e.ItemsNumber).HasColumnName("items_number");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");

            entity.HasOne(d => d.IdCreditCardNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.IdCreditCard)
                .HasConstraintName("fk_cart_credit_card");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("fk_cart_user");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => new { e.IdCart, e.IdPurchaseOffer }).HasName("cart_item_pkey");

            entity.ToTable("cart_item");

            entity.HasIndex(e => new { e.IdCart, e.IdPurchaseOffer }, "idx_cart_item_cart_offer");

            entity.HasIndex(e => e.IdPurchaseOffer, "idx_cart_item_purchase_offer");

            entity.Property(e => e.IdCart).HasColumnName("id_cart");
            entity.Property(e => e.IdPurchaseOffer).HasColumnName("id_purchase_offer");
            entity.Property(e => e.AddedAt).HasColumnName("added_at");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.ValidFrom).HasColumnName("valid_from");

            entity.HasOne(d => d.IdCartNavigation).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.IdCart)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cart_item_cart");

            entity.HasOne(d => d.IdPurchaseOfferNavigation).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.IdPurchaseOffer)
                .HasConstraintName("fk_cart_item_purchase_offer");
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

        modelBuilder.Entity<CreditCard>(entity =>
        {
            entity.HasKey(e => e.IdCreditCard).HasName("credit_card_pkey");

            entity.ToTable("credit_card");

            entity.Property(e => e.IdCreditCard).HasColumnName("id_credit_card");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Cvv)
                .HasMaxLength(255)
                .HasColumnName("cvv");
            entity.Property(e => e.ExpirationDate).HasColumnName("expiration_date");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Number)
                .HasMaxLength(255)
                .HasColumnName("number");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.CreditCards)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("fk_credit_card_user");
        });

        modelBuilder.Entity<IndividualTicket>(entity =>
        {
            entity.HasKey(e => e.IdPurchaseOffer).HasName("individual_ticket_pkey");

            entity.ToTable("individual_ticket");

            entity.HasIndex(e => e.IdMatch, "idx_individual_ticket_match");

            entity.HasIndex(e => e.IdPurchaseOffer, "idx_individual_ticket_purchase_offer");

            entity.HasIndex(e => e.IdIndividualTicket, "individual_ticket_id_individual_ticket_key").IsUnique();

            entity.Property(e => e.IdPurchaseOffer)
                .ValueGeneratedNever()
                .HasColumnName("id_purchase_offer");
            entity.Property(e => e.IdIndividualTicket)
                .ValueGeneratedOnAdd()
                .HasColumnName("id_individual_ticket");
            entity.Property(e => e.IdMatch).HasColumnName("id_match");

            entity.HasOne(d => d.IdMatchNavigation).WithMany(p => p.IndividualTickets)
                .HasForeignKey(d => d.IdMatch)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_individual_ticket_match");

            entity.HasOne(d => d.IdPurchaseOfferNavigation).WithOne(p => p.IndividualTicket)
                .HasForeignKey<IndividualTicket>(d => d.IdPurchaseOffer)
                .HasConstraintName("fk_individual_ticket_purchase_offer");
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

        modelBuilder.Entity<MatchSummaryReportView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("match_summary_report_view");

            entity.Property(e => e.AverageTicketPrice)
                .HasPrecision(10, 2)
                .HasColumnName("average_ticket_price");
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city");
            entity.Property(e => e.CompetitionName)
                .HasMaxLength(255)
                .HasColumnName("competition_name");
            entity.Property(e => e.Hall)
                .HasMaxLength(255)
                .HasColumnName("hall");
            entity.Property(e => e.HighestSellingZone)
                .HasMaxLength(255)
                .HasColumnName("highest_selling_zone");
            entity.Property(e => e.LowestSellingZone)
                .HasMaxLength(255)
                .HasColumnName("lowest_selling_zone");
            entity.Property(e => e.MatchDate).HasColumnName("match_date");
            entity.Property(e => e.MatchId).HasColumnName("match_id");
            entity.Property(e => e.MatchName)
                .HasMaxLength(255)
                .HasColumnName("match_name");
            entity.Property(e => e.MatchType)
                .HasMaxLength(20)
                .HasColumnName("match_type");
            entity.Property(e => e.OpponentPoints).HasColumnName("opponent_points");
            entity.Property(e => e.OurPoints).HasColumnName("our_points");
            entity.Property(e => e.RegularZoneRevenue)
                .HasPrecision(12, 2)
                .HasColumnName("regular_zone_revenue");
            entity.Property(e => e.RegularZoneTickets).HasColumnName("regular_zone_tickets");
            entity.Property(e => e.SeasonName)
                .HasMaxLength(255)
                .HasColumnName("season_name");
            entity.Property(e => e.StadiumFillPercentage)
                .HasPrecision(5, 2)
                .HasColumnName("stadium_fill_percentage");
            entity.Property(e => e.TeamName)
                .HasMaxLength(255)
                .HasColumnName("team_name");
            entity.Property(e => e.TotalRevenue)
                .HasPrecision(12, 2)
                .HasColumnName("total_revenue");
            entity.Property(e => e.TotalTicketsSold).HasColumnName("total_tickets_sold");
            entity.Property(e => e.TrackingStatus)
                .HasMaxLength(20)
                .HasColumnName("tracking_status");
            entity.Property(e => e.VipZoneRevenue)
                .HasPrecision(12, 2)
                .HasColumnName("vip_zone_revenue");
            entity.Property(e => e.VipZoneTickets).HasColumnName("vip_zone_tickets");
        });

        modelBuilder.Entity<MatchZoneSalesSummary>(entity =>
        {
            entity.HasKey(e => e.IdSummary).HasName("match_zone_sales_summary_pkey");

            entity.ToTable("match_zone_sales_summary");

            entity.HasIndex(e => new { e.IdMatch, e.IdZone }, "match_zone_sales_summary_id_match_id_zone_key").IsUnique();

            entity.Property(e => e.IdSummary).HasColumnName("id_summary");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.IdMatch).HasColumnName("id_match");
            entity.Property(e => e.IdTicketPriceParameter).HasColumnName("id_ticket_price_parameter");
            entity.Property(e => e.IdZone).HasColumnName("id_zone");
            entity.Property(e => e.TotalRevenue)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("total_revenue");
            entity.Property(e => e.TotalTicketsSold).HasColumnName("total_tickets_sold");

            entity.HasOne(d => d.IdMatchNavigation).WithMany(p => p.MatchZoneSalesSummaries)
                .HasForeignKey(d => d.IdMatch)
                .HasConstraintName("fk_match_zone_sales_match");

            entity.HasOne(d => d.IdTicketPriceParameterNavigation).WithMany(p => p.MatchZoneSalesSummaries)
                .HasForeignKey(d => d.IdTicketPriceParameter)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_match_zone_sales_price_param");

            entity.HasOne(d => d.IdZoneNavigation).WithMany(p => p.MatchZoneSalesSummaries)
                .HasForeignKey(d => d.IdZone)
                .HasConstraintName("fk_match_zone_sales_zone");
        });

        modelBuilder.Entity<PurchaseOffer>(entity =>
        {
            entity.HasKey(e => e.IdPurchaseOffer).HasName("purchase_offer_pkey");

            entity.ToTable("purchase_offer");

            entity.HasIndex(e => new { e.IdSeat, e.Type }, "idx_purchase_offer_seat_type");

            entity.HasIndex(e => new { e.IdSeat, e.Type, e.Status }, "idx_purchase_offer_seat_type_status");

            entity.Property(e => e.IdPurchaseOffer).HasColumnName("id_purchase_offer");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.IdSeat).HasColumnName("id_seat");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.ReleasedAt).HasColumnName("released_at");
            entity.Property(e => e.Status)
                .HasMaxLength(25)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");

            entity.HasOne(d => d.IdSeatNavigation).WithMany(p => p.PurchaseOffers)
                .HasForeignKey(d => d.IdSeat)
                .HasConstraintName("fk_purchase_offer_seat");
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

        modelBuilder.Entity<SeasonTicket>(entity =>
        {
            entity.HasKey(e => e.IdPurchaseOffer).HasName("season_ticket_pkey");

            entity.ToTable("season_ticket");

            entity.Property(e => e.IdPurchaseOffer)
                .ValueGeneratedNever()
                .HasColumnName("id_purchase_offer");
            entity.Property(e => e.IdSeason).HasColumnName("id_season");
            entity.Property(e => e.TicketPrice).HasColumnName("ticket_price");

            entity.HasOne(d => d.IdPurchaseOfferNavigation).WithOne(p => p.SeasonTicket)
                .HasForeignKey<SeasonTicket>(d => d.IdPurchaseOffer)
                .HasConstraintName("fk_season_ticket_purchase_offer");

            entity.HasOne(d => d.IdSeasonNavigation).WithMany(p => p.SeasonTickets)
                .HasForeignKey(d => d.IdSeason)
                .HasConstraintName("fk_season_ticket_season");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.IdSeat).HasName("seat_pkey");

            entity.ToTable("seat");

            entity.HasIndex(e => new { e.IdZone, e.Direction }, "idx_seat_zone_direction");

            entity.HasIndex(e => new { e.IdZone, e.Direction, e.Row, e.Number }, "idx_seat_zone_direction_row_number");

            entity.Property(e => e.IdSeat).HasColumnName("id_seat");
            entity.Property(e => e.Direction)
                .HasMaxLength(255)
                .HasColumnName("direction");
            entity.Property(e => e.IdZone).HasColumnName("id_zone");
            entity.Property(e => e.Number).HasColumnName("number");
            entity.Property(e => e.Row).HasColumnName("row");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");

            entity.HasOne(d => d.IdZoneNavigation).WithMany(p => p.Seats)
                .HasForeignKey(d => d.IdZone)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_seat_zone");
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

        modelBuilder.Entity<TicketPriceParameter>(entity =>
        {
            entity.HasKey(e => e.IdTicketPriceParameter).HasName("ticket_price_parameter_pkey");

            entity.ToTable("ticket_price_parameter");

            entity.HasIndex(e => new { e.IdMatch, e.IdZone }, "idx_ticket_price_parameter_match_zone");

            entity.Property(e => e.IdTicketPriceParameter).HasColumnName("id_ticket_price_parameter");
            entity.Property(e => e.IdMatch).HasColumnName("id_match");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.IdZone).HasColumnName("id_zone");
            entity.Property(e => e.MaximumSeatPrice).HasColumnName("maximum_seat_price");
            entity.Property(e => e.MinimumSeatPrice).HasColumnName("minimum_seat_price");
            entity.Property(e => e.PriceFactor).HasColumnName("price_factor");
            entity.Property(e => e.TimeFactor).HasColumnName("time_factor");

            entity.HasOne(d => d.IdMatchNavigation).WithMany(p => p.TicketPriceParameters)
                .HasForeignKey(d => d.IdMatch)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ticket_price_param_match");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.TicketPriceParameters)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ticket_price_param_user");

            entity.HasOne(d => d.IdZoneNavigation).WithMany(p => p.TicketPriceParameters)
                .HasForeignKey(d => d.IdZone)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ticket_price_param_zone");
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

        modelBuilder.Entity<Zone>(entity =>
        {
            entity.HasKey(e => e.IdZone).HasName("zone_pkey");

            entity.ToTable("zone");

            entity.HasIndex(e => e.Status, "idx_zone_status");

            entity.Property(e => e.IdZone).HasColumnName("id_zone");
            entity.Property(e => e.MaximumCapacity).HasColumnName("maximum_capacity");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Rank).HasColumnName("rank");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
