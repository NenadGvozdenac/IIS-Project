using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Interfaces;

public interface ISeasonRepository
{
    Season? GetById(int id);
    IEnumerable<Season> GetAll();
    Season Create(Season season);
    Season Update(Season season);
    bool Delete(int id);
    bool ExistsByName(string name);
    bool ExistsByName(string name, int excludeId);
}
