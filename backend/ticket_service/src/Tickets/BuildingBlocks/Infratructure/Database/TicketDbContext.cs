using Microsoft.EntityFrameworkCore;

namespace ticket_service.src.Tickets.BuildingBlocks.Infratructure.Database;

public class TicketDbContext : DbContext
{
    public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Add your entity configurations here
        // Since you're using database-first, you can scaffold entities using:
        // dotnet ef dbcontext scaffold "Host=postgres_db;Database=sportsdb;Username=postgres;Password=postgres;Port=5432" Npgsql.EntityFrameworkCore.PostgreSQL
    }
}
