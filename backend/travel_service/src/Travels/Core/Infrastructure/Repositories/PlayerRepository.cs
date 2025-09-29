using Microsoft.EntityFrameworkCore;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;
using travel_service.src.Travels.Core.Infrastructure;

namespace travel_service.src.Travels.Core.Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly TravelDbContext _travelDbContext;

    public PlayerRepository(TravelDbContext context)
    {
        _travelDbContext = context;
    }

    public IEnumerable<Player> GetAll()
    {
        return _travelDbContext.Players.ToList();
    }

    public Player? GetById(int id)
    {
        return _travelDbContext.Players.FirstOrDefault(p => p.IdPlayer == id);
    }
}
