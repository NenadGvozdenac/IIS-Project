using Microsoft.EntityFrameworkCore;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Infrastructure;

namespace scouting_service.src.Scoutings.Core.Infrastructure.Repositories;

public class SessionTypeRepository : ISessionTypeRepository
{
    private readonly ScoutingDbContext _scoutingDbContext;

    public SessionTypeRepository(ScoutingDbContext context)
    {
        _scoutingDbContext = context;
    }

    public SessionType? GetById(int id)
    {
        return _scoutingDbContext.SessionTypes
            .Include(st => st.Sessions)
            .FirstOrDefault(st => st.IdSessionType == id);
    }

    public IEnumerable<SessionType> GetAll()
    {
        return _scoutingDbContext.SessionTypes
            .Include(st => st.Sessions)
            .ToList();
    }

    public SessionType? GetByName(string name)
    {
        return _scoutingDbContext.SessionTypes
            .Include(st => st.Sessions)
            .FirstOrDefault(st => st.Name == name);
    }

    public SessionType Create(SessionType sessionType)
    {
        _scoutingDbContext.SessionTypes.Add(sessionType);
        _scoutingDbContext.SaveChanges();
        return sessionType;
    }

    public SessionType Update(SessionType sessionType)
    {
        _scoutingDbContext.SessionTypes.Update(sessionType);
        _scoutingDbContext.SaveChanges();
        return sessionType;
    }

    public bool Delete(int id)
    {
        var sessionType = _scoutingDbContext.SessionTypes.Find(id);
        if (sessionType == null) return false;

        _scoutingDbContext.SessionTypes.Remove(sessionType);
        _scoutingDbContext.SaveChanges();
        return true;
    }
}
