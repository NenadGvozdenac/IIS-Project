using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Interfaces;

public interface ICompetitionRepository
{
    IEnumerable<Competition> GetAll();
}
