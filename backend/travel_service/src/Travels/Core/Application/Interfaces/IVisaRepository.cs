using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Interfaces;

public interface IVisaRepository
{
    IEnumerable<Visa> GetByTravelInformationId(int id);
    Visa Create(Visa visa);
}