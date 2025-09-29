using Travel_Service.src.Travels.Core.Domain.ColumnarEntities;

namespace Travel_Service.src.Travels.Core.Application.Interfaces.ColumnarRepositories;

public interface IColumnarRepository<T> where T : class
{
    Task<T?> GetByIdAsync(params object[] keys);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetByPartitionKeyAsync(object partitionKey);
    Task<bool> CreateAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(params object[] keys);
    Task<bool> ExistsAsync(params object[] keys);
}

public interface IMatchRepository : IColumnarRepository<Match>
{
}

public interface IRequestRepository : IColumnarRepository<Request>
{
}

public interface IAccommodationRequestRepository : IColumnarRepository<AccommodationRequest>
{
}

public interface ITransportationRequestRepository : IColumnarRepository<TransportationRequest>
{
}

public interface IOfferRepository : IColumnarRepository<Offer>
{
}

public interface IAccommodationOfferRepository : IColumnarRepository<AccommodationOffer>
{
}

public interface ITransportationOfferRepository : IColumnarRepository<TransportationOffer>
{
}

public interface ITripRepository : IColumnarRepository<Trip>
{
}