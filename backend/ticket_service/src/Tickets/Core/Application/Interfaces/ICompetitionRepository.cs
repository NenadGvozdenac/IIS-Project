using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Interfaces;

public interface ICompetitionRepository
{
    IEnumerable<Competition> GetAll();
    Competition? GetById(int id);
    Competition Create(Competition competition);
    Competition Update(Competition competition);
    void Delete(int id);
    bool ExistsByName(string name);
    bool ExistsByNameExcludingId(string name, int id);
    IEnumerable<Competition> GetActiveCompetitions();
    IEnumerable<Competition> GetCompetitionsByDateRange(DateOnly startDate, DateOnly endDate);
}
