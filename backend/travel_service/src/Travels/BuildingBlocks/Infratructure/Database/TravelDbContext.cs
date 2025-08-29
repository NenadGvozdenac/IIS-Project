using Microsoft.EntityFrameworkCore;
using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.BuildingBlocks.Infratructure.Database;

public class TravelDbContext : DbContext
{
    public TravelDbContext(DbContextOptions<TravelDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Add your entity configurations here
        // Since you're using database-first, you can scaffold entities using:
        // dotnet ef dbcontext scaffold "Host=postgres_db;Database=sportsdb;Username=postgres;Password=postgres;Port=5432" Npgsql.EntityFrameworkCore.PostgreSQL
    }
}
