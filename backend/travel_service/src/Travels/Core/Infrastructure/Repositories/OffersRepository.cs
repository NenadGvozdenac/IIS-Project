using Microsoft.EntityFrameworkCore;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;
using travel_service.src.Travels.Core.Infrastructure;
using travel_service.src.Travels.Core.Application.Commands.AutoSelectBestOffer;
using Npgsql;

namespace travel_service.src.Travels.Core.Infrastructure.Repositories;

public class OffersRepository : IOffersRepository
{
    private readonly TravelDbContext _travelDbContext;

    public OffersRepository(TravelDbContext context)
    {
        _travelDbContext = context;
    }

    public IEnumerable<Offer> GetAllOffers()
    {
        return _travelDbContext.Offers
            .Include(o => o.AccommodationOffer)
            .Include(o => o.TransportationOffer)
            .Include(o => o.IdMatchNavigation)
            .Include(o => o.UserIdUserNavigation)
            .ToList();
    }

    public IEnumerable<Offer> GetOffersByType(string type)
    {
        return _travelDbContext.Offers
            .Include(o => o.AccommodationOffer)
            .Include(o => o.TransportationOffer)
            .Include(o => o.IdMatchNavigation)
            .Include(o => o.UserIdUserNavigation)
            .Where(o => o.Type == type)
            .ToList();
    }

    public IEnumerable<Offer> GetOffersByTypeAndMatch(string type, int idMatch)
    {
        return _travelDbContext.Offers
            .Include(o => o.AccommodationOffer)
            .Include(o => o.TransportationOffer)
            .Include(o => o.IdMatchNavigation)
            .Include(o => o.UserIdUserNavigation)
            .Include(o => o.Id.IdAgencyNavigation) 
            .Where(o => o.Type == type && o.IdMatch == idMatch)
            .ToList();
    }

    public Offer? GetChosenOfferByTypeAndMatch(string type, int idMatch)
    {
        return _travelDbContext.Offers
            .Include(o => o.AccommodationOffer)
            .Include(o => o.TransportationOffer)
            .Include(o => o.IdMatchNavigation)
            .Include(o => o.UserIdUserNavigation)
            .Include(o => o.Id.IdAgencyNavigation) 
            .Where(o => o.Type == type && o.IdMatch == idMatch && o.Chosen == true)
            .FirstOrDefault();
    }

    public Offer? GetOfferById(int idOffer)
    {
        return _travelDbContext.Offers
            .Include(o => o.AccommodationOffer)
            .Include(o => o.TransportationOffer)
            .Include(o => o.IdMatchNavigation)
            .Include(o => o.UserIdUserNavigation)
            .Where(o => o.IdOffer == idOffer)
            .FirstOrDefault();
    }

    public Offer? GetOfferByCompositeKey(int idOffer, int idAgency, int idRequest)
    {
        return _travelDbContext.Offers
            .Include(o => o.AccommodationOffer)
            .Include(o => o.TransportationOffer)
            .Include(o => o.IdMatchNavigation)
            .Include(o => o.UserIdUserNavigation)
            .Where(o => o.IdOffer == idOffer && o.IdAgency == idAgency && o.IdRequest == idRequest)
            .FirstOrDefault();
    }

    public Offer CreateOffer(Offer offer)
    {
        try
        {
            var maxIdOffer = _travelDbContext.Offers
                .Where(o => o.IdAgency == offer.IdAgency && o.IdRequest == offer.IdRequest)
                .Max(o => (int?)o.IdOffer) ?? 0;
            
            offer.IdOffer = maxIdOffer + 1;

            _travelDbContext.Database.ExecuteSqlRaw(
                @"INSERT INTO offer (id_offer, price, user_id_user, id_match, id_agency, id_request, chosen, type) 
                  VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})",
                offer.IdOffer,
                offer.Price ?? (object)DBNull.Value,
                offer.UserIdUser ?? (object)DBNull.Value,
                offer.IdMatch,
                offer.IdAgency,
                offer.IdRequest,
                offer.Chosen ?? false,
                offer.Type ?? (object)DBNull.Value);

            if (offer.Type == "accommodation" && offer.AccommodationOffer != null)
            {
                var accommodation = offer.AccommodationOffer;
                accommodation.IdOffer = offer.IdOffer;
                
                _travelDbContext.Database.ExecuteSqlRaw(
                    @"INSERT INTO accommodation_offer 
                      (id_offer, name, capacity, accommodation_type, id_agency, id_request, 
                       double_room, triple_room, quadruple_room, breakfast, fitness_center, 
                       pool, wifi, spa) 
                      VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13})",
                    accommodation.IdOffer,
                    accommodation.Name ?? (object)DBNull.Value, 
                    accommodation.Capacity ?? (object)DBNull.Value, 
                    accommodation.AccommodationType ?? (object)DBNull.Value, 
                    accommodation.IdAgency, 
                    accommodation.IdRequest,
                    accommodation.DoubleRoom, 
                    accommodation.TripleRoom, 
                    accommodation.QuadrupleRoom,
                    accommodation.Breakfast, 
                    accommodation.FitnessCenter, 
                    accommodation.Pool,
                    accommodation.Wifi, 
                    accommodation.Spa);
            }
            else if (offer.Type == "transportation" && offer.TransportationOffer != null)
            {
                var transportation = offer.TransportationOffer;
                transportation.IdOffer = offer.IdOffer;
                
                _travelDbContext.Database.ExecuteSqlRaw(
                    @"INSERT INTO transportation_offer 
                      (id_offer, company_name, capacity, type, id_agency, id_request, 
                       equipment_space, air_conditioning, tv, wifi, restroom) 
                      VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10})",
                    transportation.IdOffer,
                    transportation.CompanyName ?? (object)DBNull.Value, 
                    transportation.Capacity ?? (object)DBNull.Value,
                    transportation.Type ?? (object)DBNull.Value, 
                    transportation.IdAgency, 
                    transportation.IdRequest,
                    transportation.EquipmentSpace, 
                    transportation.AirConditioning, 
                    transportation.Tv, 
                    transportation.Wifi, 
                    transportation.Restroom);
            }

            return offer;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating offer: {ex.Message}", ex);
        }
    }

    public void UpdateOfferStatus(int idOffer, int idAgency, int idRequest, bool chosen)
    {
        try
        {
            // Ako se postavlja chosen = true, prvo treba da postavimo sve ostale ponude istog tipa i meča na false
            if (chosen)
            {
                // Prvo dobij tip i match ID trenutne ponude
                var currentOffer = _travelDbContext.Offers
                    .Where(o => o.IdOffer == idOffer && o.IdAgency == idAgency && o.IdRequest == idRequest)
                    .FirstOrDefault();

                if (currentOffer != null)
                {
                    // Postavi sve ostale ponude istog tipa i meča na chosen = false
                    _travelDbContext.Database.ExecuteSqlRaw(
                        @"UPDATE offer SET chosen = false 
                          WHERE type = {0} AND id_match = {1} AND 
                          NOT (id_offer = {2} AND id_agency = {3} AND id_request = {4})",
                        currentOffer.Type ?? (object)DBNull.Value,
                        currentOffer.IdMatch,
                        idOffer,
                        idAgency,
                        idRequest);
                }
            }

            // Ažuriraj status trenutne ponude
            _travelDbContext.Database.ExecuteSqlRaw(
                @"UPDATE offer SET chosen = {0} 
                  WHERE id_offer = {1} AND id_agency = {2} AND id_request = {3}",
                chosen,
                idOffer,
                idAgency,
                idRequest);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating offer status: {ex.Message}", ex);
        }
    }

    public async Task<AutoSelectBestOfferResponse?> AutoSelectBestOffer(int matchId, string offerType, decimal weightPrice, decimal weightCapacity, decimal weightBenefits, decimal weightAgency)
    {
        try
        {
            var parameters = new[]
            {
                new NpgsqlParameter("p_match_id", matchId),
                new NpgsqlParameter("p_offer_type", offerType),
                new NpgsqlParameter("p_weight_price", weightPrice),
                new NpgsqlParameter("p_weight_capacity", weightCapacity),
                new NpgsqlParameter("p_weight_benefits", weightBenefits),
                new NpgsqlParameter("p_weight_agency", weightAgency)
            };

            var sql = "SELECT * FROM auto_select_best_offer(@p_match_id, @p_offer_type, @p_weight_price, @p_weight_capacity, @p_weight_benefits, @p_weight_agency)";

            using var command = _travelDbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddRange(parameters);

            await _travelDbContext.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                var selectedOfferId = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0);
                var selectedAgencyName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                var selectionScore = reader.IsDBNull(2) ? 0m : reader.GetDecimal(2);
                var selectionReason = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                var totalOffersAnalyzed = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);

                return new AutoSelectBestOfferResponse
                {
                    SelectedOfferId = selectedOfferId,
                    SelectedAgencyName = selectedAgencyName,
                    SelectionScore = selectionScore,
                    SelectionReason = selectionReason,
                    TotalOffersAnalyzed = totalOffersAnalyzed
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in auto-select best offer: {ex.Message}", ex);
        }
    }

}
