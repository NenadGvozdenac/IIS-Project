using Microsoft.EntityFrameworkCore;
using match_service.src.Matches.Core.Domain.Entities;

namespace match_service.src.Matches.BuildingBlocks.Infratructure.Database;

public class MatchDbContext : DbContext
{
    public MatchDbContext(DbContextOptions<MatchDbContext> options) : base(options)
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
