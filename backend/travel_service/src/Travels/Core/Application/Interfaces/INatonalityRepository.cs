using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Interfaces;

public interface INationalityRepository
{
    Nationality? GetById(int id);
    IEnumerable<Nationality> GetAll();
}
