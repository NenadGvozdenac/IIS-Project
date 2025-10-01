using Microsoft.AspNetCore.Mvc;
using Travel_Service.src.Travels.Core.Application.Interfaces.ColumnarRepositories;
using Travel_Service.src.Travels.Core.Domain.ColumnarEntities;
using Cassandra;

namespace travel_service.src.Travels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CassandraCrudController : ControllerBase
{
    private readonly IMatchRepository _matchRepository;
    private readonly IOfferRepository _offerRepository;
    private readonly IRequestRepository _requestRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IAccommodationOfferRepository _accommodationOfferRepository;
    private readonly IAccommodationRequestRepository _accommodationRequestRepository;
    private readonly ITransportationOfferRepository _transportationOfferRepository;
    private readonly ITransportationRequestRepository _transportationRequestRepository;
    private readonly Cassandra.ISession _session;

    public CassandraCrudController(
        IMatchRepository matchRepository,
        IOfferRepository offerRepository,
        IRequestRepository requestRepository,
        ITripRepository tripRepository,
        IAccommodationOfferRepository accommodationOfferRepository,
        IAccommodationRequestRepository accommodationRequestRepository,
        ITransportationOfferRepository transportationOfferRepository,
        ITransportationRequestRepository transportationRequestRepository,
        Cassandra.ISession session)
    {
        _matchRepository = matchRepository;
        _offerRepository = offerRepository;
        _requestRepository = requestRepository;
        _tripRepository = tripRepository;
        _accommodationOfferRepository = accommodationOfferRepository;
        _accommodationRequestRepository = accommodationRequestRepository;
        _transportationOfferRepository = transportationOfferRepository;
        _transportationRequestRepository = transportationRequestRepository;
        _session = session;
    }

    
    [HttpGet("health")]
    public async Task<IActionResult> Health()
    {
        try
        {
            await _matchRepository.GetAllAsync();
            await _offerRepository.GetAllAsync();
            await _requestRepository.GetAllAsync();
            await _tripRepository.GetAllAsync();
            await _accommodationOfferRepository.GetAllAsync();
            await _accommodationRequestRepository.GetAllAsync();
            await _transportationOfferRepository.GetAllAsync();
            await _transportationRequestRepository.GetAllAsync();
            
            return Ok(new { status = "healthy", message = "All Cassandra repositories are accessible" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("matches")]
    public async Task<IActionResult> CreateMatch([FromBody] Match match)
    {
        try
        {
            await _matchRepository.CreateAsync(match);
            return Ok(new { message = "Match created successfully", data = match });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("matches")]
    public async Task<IActionResult> GetAllMatches()
    {
        try
        {
            var matches = await _matchRepository.GetAllAsync();
            return Ok(matches);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("matches/{idMatch}")]
    public async Task<IActionResult> UpdateMatch(int idMatch, [FromBody] Match updatedMatch)
    {
        try
        {
            var matches = await _matchRepository.GetAllAsync();
            var existingMatch = matches.FirstOrDefault(m => m.IdMatch == idMatch);
            
            if (existingMatch == null)
                return NotFound(new { error = "Match not found" });

            if (DeleteMatch(existingMatch.IdMatch) == null)
            {
                return BadRequest(new { error = "Failed to delete existing match before update" });
            }

            existingMatch.City = updatedMatch.City;
            existingMatch.Name = updatedMatch.Name;
            existingMatch.State = updatedMatch.State;
            existingMatch.Hall = updatedMatch.Hall;
            existingMatch.ScheduledAt = updatedMatch.ScheduledAt;
            existingMatch.Type = updatedMatch.Type;
            existingMatch.IsInOurHall = updatedMatch.IsInOurHall;
            existingMatch.TransportationRequired = updatedMatch.TransportationRequired;
            existingMatch.AccommodationRequired = updatedMatch.AccommodationRequired;

            await _matchRepository.UpdateAsync(existingMatch);
            return Ok(new { message = "Match updated successfully", data = existingMatch });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    [HttpDelete("matches/{idMatch}")]
    public async Task<IActionResult> DeleteMatch(int idMatch)
    {
        try
        {
            var matches = await _matchRepository.GetAllAsync();
            var existingMatch = matches.FirstOrDefault(m => m.IdMatch == idMatch);
            if (existingMatch == null)
                return NotFound(new { error = "Match not found" });

            var array = new object[] { 
                existingMatch.City,
                existingMatch.Type,
                existingMatch.ScheduledAt,
                existingMatch.IdMatch
             };

            Console.WriteLine($"Deleting match: {existingMatch.IdMatch}");
            await _matchRepository.DeleteAsync(array);
            return Ok(new { message = "Match deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("offers")]
    public async Task<IActionResult> CreateOffer([FromBody] Offer offer)
    {
        try
        {
            var array = new object[] { offer.IdMatch, offer.IdRequest };
            var requestExists = await _requestRepository.ExistsAsync(array);
            if (!requestExists)
                return NotFound(new { error = "Request not found" });

            Console.WriteLine($"Creating offer for Match ID: {offer.IdMatch}, Offer ID: {offer.IdOffer}, Request ID: {offer.IdRequest}, Offer type: {offer.Type}");
            await _offerRepository.CreateAsync(offer);
            return Ok(new { message = "Offer created successfully", data = offer });
        }
        catch (Exception ex){
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("offers")]
    public async Task<IActionResult> GetAllOffers()
    {
        try
        {
            var offers = await _offerRepository.GetAllAsync();
            return Ok(offers);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("offers/{idMatch}/{idOffer}/{idRequest}")]
    public async Task<IActionResult> UpdateOffer(int idMatch, int idOffer, int idRequest, [FromBody] Offer updatedOffer)
    {
        try
        {
            var offers = await _offerRepository.GetAllAsync();
            var existingOffer = offers.FirstOrDefault(o => o.IdMatch == idMatch && o.IdOffer == idOffer && o.IdRequest == idRequest);
            
            if (existingOffer == null)
                return NotFound(new { error = "Offer not found" });

            if (DeleteOffer(existingOffer.IdMatch, existingOffer.IdOffer, existingOffer.IdRequest) == null)
            {
                return BadRequest(new { error = "Failed to delete existing offer before update" });
            }

            existingOffer.Type = updatedOffer.Type;
            existingOffer.Price = updatedOffer.Price;
            existingOffer.UserIdUser = updatedOffer.UserIdUser;
            existingOffer.Chosen = updatedOffer.Chosen;
            existingOffer.Score = updatedOffer.Score;

            await _offerRepository.UpdateAsync(existingOffer);
            return Ok(new { message = "Offer updated successfully", data = existingOffer });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("offers/{idMatch}/{idOffer}/{idRequest}")]
    public async Task<IActionResult> DeleteOffer(int idMatch, int idOffer, int idRequest)
    {
        try
        {
            var offers = await _offerRepository.GetAllAsync();
            var existingOffer = offers.FirstOrDefault(o => o.IdMatch == idMatch && o.IdOffer == idOffer && o.IdRequest == idRequest);
            
            if (existingOffer == null)
                return NotFound(new { error = "Offer not found" });

            Console.WriteLine($"Deleting offer: Match ID {existingOffer.IdMatch}, Offer ID {existingOffer.IdOffer}, Request ID {existingOffer.IdRequest}, {existingOffer.Type}, {existingOffer.Chosen}, {existingOffer.Price}");
            
            var array = new object[] {
                existingOffer.IdMatch,
                existingOffer.Type,
                existingOffer.Chosen,
                existingOffer.Price,
                existingOffer.IdOffer
            };
            var query = "DELETE FROM offer WHERE id_match = ? AND type = ? AND chosen = ? AND price = ? AND id_offer = ?";
            await _session.ExecuteAsync(
                new SimpleStatement(query, existingOffer.IdMatch, existingOffer.Type, existingOffer.Chosen, existingOffer.Price, existingOffer.IdOffer)
            );
            //await _offerRepository.DeleteAsync(array);
            return Ok(new { message = "Offer deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    
    [HttpPost("requests")]
    public async Task<IActionResult> CreateRequest([FromBody] Request request)
    {
        try
        {
            var array = new object[] { request.IdMatch };
            var requestExists = await _matchRepository.ExistsAsync(array);

            if (!requestExists)
                return NotFound(new { error = "Match not found" });

            await _requestRepository.CreateAsync(request);
            return Ok(new { message = "Request created successfully", data = request });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("requests")]
    public async Task<IActionResult> GetAllRequests()
    {
        try
        {
            var requests = await _requestRepository.GetAllAsync();
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("requests/{idRequest}")]
    public async Task<IActionResult> UpdateRequest(int idRequest, [FromBody] Request updatedRequest)
    {
        try
        {
            var requests = await _requestRepository.GetAllAsync();
            var existingRequest = requests.FirstOrDefault(r => r.IdRequest == idRequest);
            
            if (existingRequest == null)
                return NotFound(new { error = "Request not found" });

            if (DeleteRequest(existingRequest.IdRequest) == null)
            {
                return BadRequest(new { error = "Failed to delete existing request before update" });
            }

            existingRequest.Type = updatedRequest.Type;
            existingRequest.Budget = updatedRequest.Budget;
            existingRequest.State = updatedRequest.State;
            existingRequest.City = updatedRequest.City;
            existingRequest.Hall = updatedRequest.Hall;

            await _requestRepository.UpdateAsync(existingRequest);
            return Ok(new { message = "Request updated successfully", data = existingRequest });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("requests/{idRequest}")]
    public async Task<IActionResult> DeleteRequest(int idRequest)
    {
        try
        {
            var requests = await _requestRepository.GetAllAsync();
            var existingRequest = requests.FirstOrDefault(r => r.IdRequest == idRequest);
            
            if (existingRequest == null)
                return NotFound(new { error = "Request not found" });

            var array = new object[] { 
                existingRequest.IdMatch,
                existingRequest.Type,
                existingRequest.Budget,
                existingRequest.IdRequest
             };

            await _requestRepository.DeleteAsync(array);
            return Ok(new { message = "Request deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    
    [HttpPost("trips")]
    public async Task<IActionResult> CreateTrip([FromBody] Trip trip)
    {
        try
        {
            await _tripRepository.CreateAsync(trip);
            return Ok(new { message = "Trip created successfully", data = trip });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("trips")]
    public async Task<IActionResult> GetAllTrips()
    {
        try
        {
            var trips = await _tripRepository.GetAllAsync();
            return Ok(trips);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("trips/{idTrip}")]
    public async Task<IActionResult> UpdateTrip(int idTrip, [FromBody] Trip updatedTrip)
    {
        try
        {
            var trips = await _tripRepository.GetAllAsync();
            var existingTrip = trips.FirstOrDefault(t => t.IdTrip == idTrip);
            
            if (existingTrip == null)
                return NotFound(new { error = "Trip not found" });
            if (DeleteTrip(existingTrip.IdTrip) == null)
            {
                return BadRequest(new { error = "Failed to delete existing trip before update" });
            }


            existingTrip.Notes = updatedTrip.Notes;
            existingTrip.IdAccommodationOffer = updatedTrip.IdAccommodationOffer;
            existingTrip.IdTransportationOffer = updatedTrip.IdTransportationOffer;
            existingTrip.IdAccommodationRequest = updatedTrip.IdAccommodationRequest;
            existingTrip.IdTransportationRequest = updatedTrip.IdTransportationRequest;

            await _tripRepository.UpdateAsync(existingTrip);
            return Ok(new { message = "Trip updated successfully", data = existingTrip });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("trips/{idTrip}")]
    public async Task<IActionResult> DeleteTrip(int idTrip)
    {
        try
        {
            var trips = await _tripRepository.GetAllAsync();
            var existingTrip = trips.FirstOrDefault(t => t.IdTrip == idTrip);
            
            if (existingTrip == null)
                return NotFound(new { error = "Trip not found" });

            var array = new object[] { 
                existingTrip.MatchIdMatch,
                existingTrip.IdTrip
             };
            await _tripRepository.DeleteAsync(array);
            return Ok(new { message = "Trip deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    
    [HttpPost("accommodation-requests")]
    public async Task<IActionResult> CreateAccommodationRequest([FromBody] AccommodationRequest accommodationRequest)
    {
        try
        {
            var array = new object[] { accommodationRequest.IdRequest };
            var requestExists = await _requestRepository.ExistsAsync(array);
            if (!requestExists)
                return NotFound(new { error = "Accommodation request not found" });

            await _accommodationRequestRepository.CreateAsync(accommodationRequest);
            return Ok(new { message = "Accommodation request created successfully", data = accommodationRequest });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("accommodation-requests")]
    public async Task<IActionResult> GetAllAccommodationRequests()
    {
        try
        {
            var requests = await _accommodationRequestRepository.GetAllAsync();
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("accommodation-requests/{idRequest}")]
    public async Task<IActionResult> UpdateAccommodationRequest(int idRequest, [FromBody] AccommodationRequest updatedRequest)
    {
        try
        {
            var requests = await _accommodationRequestRepository.GetAllAsync();
            var existingRequest = requests.FirstOrDefault(r => r.IdRequest == idRequest);
            
            if (existingRequest == null)
                return NotFound(new { error = "Accommodation request not found" });
            if (DeleteAccommodationRequest(existingRequest.IdRequest) == null)
            {
                return BadRequest(new { error = "Failed to delete existing request before update" });
            }

            existingRequest.AccommodationType = updatedRequest.AccommodationType;
            existingRequest.CheckInDate = updatedRequest.CheckInDate;
            existingRequest.CheckOutDate = updatedRequest.CheckOutDate;
            existingRequest.NumberOfGuests = updatedRequest.NumberOfGuests;
            existingRequest.NumberOfRooms = updatedRequest.NumberOfRooms;

            await _accommodationRequestRepository.UpdateAsync(existingRequest);
            return Ok(new { message = "Accommodation request updated successfully", data = existingRequest });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("accommodation-requests/{idRequest}")]
    public async Task<IActionResult> DeleteAccommodationRequest(int idRequest)
    {
        try
        {
            var requests = await _accommodationRequestRepository.GetAllAsync();
            var existingRequest = requests.FirstOrDefault(r => r.IdRequest == idRequest);
            
            if (existingRequest == null)
                return NotFound(new { error = "Accommodation request not found" });

            var array = new object[] { 
                existingRequest.AccommodationType,
                existingRequest.CheckInDate,
                existingRequest.IdRequest
             };
            await _accommodationRequestRepository.DeleteAsync(array);
            return Ok(new { message = "Accommodation request deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    
    [HttpPost("transportation-requests")]
    public async Task<IActionResult> CreateTransportationRequest([FromBody] TransportationRequest transportationRequest)
    {
        try
        {
            var array = new object[] { transportationRequest.IdRequest };
            var requestExists = await _requestRepository.ExistsAsync(array);
            if (!requestExists)
                return NotFound(new { error = "Transportation request not found" });

            await _transportationRequestRepository.CreateAsync(transportationRequest);
            return Ok(new { message = "Transportation request created successfully", data = transportationRequest });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("transportation-requests")]
    public async Task<IActionResult> GetAllTransportationRequests()
    {
        try
        {
            var requests = await _transportationRequestRepository.GetAllAsync();
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("transportation-requests/{idRequest}")]
    public async Task<IActionResult> UpdateTransportationRequest(int idRequest, [FromBody] TransportationRequest updatedRequest)
    {
        try
        {
            var requests = await _transportationRequestRepository.GetAllAsync();
            var existingRequest = requests.FirstOrDefault(r => r.IdRequest == idRequest);
            
            if (existingRequest == null)
                return NotFound(new { error = "Transportation request not found" });
            if (DeleteTransportationRequest(existingRequest.IdRequest) == null)
            {
                return BadRequest(new { error = "Failed to delete existing request before update" });
            }   

            existingRequest.VehicleType = updatedRequest.VehicleType;
            existingRequest.StartDate = updatedRequest.StartDate;
            existingRequest.EndDate = updatedRequest.EndDate;
            existingRequest.NumberOfPassengers = updatedRequest.NumberOfPassengers;

            await _transportationRequestRepository.UpdateAsync(existingRequest);
            return Ok(new { message = "Transportation request updated successfully", data = existingRequest });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("transportation-requests/{idRequest}")]
    public async Task<IActionResult> DeleteTransportationRequest(int idRequest)
    {
        try
        {
            var requests = await _transportationRequestRepository.GetAllAsync();
            var existingRequest = requests.FirstOrDefault(r => r.IdRequest == idRequest);
            
            if (existingRequest == null)
                return NotFound(new { error = "Transportation request not found" });

            var array = new object[] { 
                existingRequest.VehicleType,
                existingRequest.StartDate,
                existingRequest.IdRequest
            };
            await _transportationRequestRepository.DeleteAsync(array);
            return Ok(new { message = "Transportation request deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

   
    [HttpPost("accommodation-offers")]
    public async Task<IActionResult> CreateAccommodationOffer([FromBody] AccommodationOffer accommodationOffer)
    {
        try
        {
            var array = new object[] { accommodationOffer.IdRequest, accommodationOffer.IdOffer };
            var offerExists = await _offerRepository.ExistsAsync(array);
            if (!offerExists)
                return NotFound(new { error = "Accommodation offer not found" });

            await _accommodationOfferRepository.CreateAsync(accommodationOffer);
            return Ok(new { message = "Accommodation offer created successfully", data = accommodationOffer });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("accommodation-offers")]
    public async Task<IActionResult> GetAllAccommodationOffers()
    {
        try
        {
            var offers = await _accommodationOfferRepository.GetAllAsync();
            return Ok(offers);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("accommodation-offers/{idOffer}/{idRequest}")]
    public async Task<IActionResult> UpdateAccommodationOffer(int idOffer, int idRequest, [FromBody] AccommodationOffer updatedOffer)
    {
        try
        {
            var offers = await _accommodationOfferRepository.GetAllAsync();
            var existingOffer = offers.FirstOrDefault(o => o.IdOffer == idOffer && o.IdRequest == idRequest);
            
            if (existingOffer == null)
                return NotFound(new { error = "Accommodation offer not found" });
            if (DeleteAccommodationOffer(existingOffer.IdOffer, existingOffer.IdRequest) == null)
            {
                return BadRequest(new { error = "Failed to delete existing offer before update" });
            }

            existingOffer.AccommodationType = updatedOffer.AccommodationType;
            existingOffer.Capacity = updatedOffer.Capacity;
            existingOffer.Name = updatedOffer.Name;
            existingOffer.DoubleRoom = updatedOffer.DoubleRoom;
            existingOffer.TripleRoom = updatedOffer.TripleRoom;
            existingOffer.QuadrupleRoom = updatedOffer.QuadrupleRoom;
            existingOffer.Breakfast = updatedOffer.Breakfast;
            existingOffer.FitnessCenter = updatedOffer.FitnessCenter;
            existingOffer.Pool = updatedOffer.Pool;
            existingOffer.Wifi = updatedOffer.Wifi;
            existingOffer.Spa = updatedOffer.Spa;

            await _accommodationOfferRepository.UpdateAsync(existingOffer);
            return Ok(new { message = "Accommodation offer updated successfully", data = existingOffer });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("accommodation-offers/{idOffer}/{idRequest}")]
    public async Task<IActionResult> DeleteAccommodationOffer(int idOffer, int idRequest)
    {
        try
        {
            var offers = await _accommodationOfferRepository.GetAllAsync();
            var existingOffer = offers.FirstOrDefault(o => o.IdOffer == idOffer && o.IdRequest == idRequest);
            
            if (existingOffer == null)
                return NotFound(new { error = "Accommodation offer not found" });

            var array = new object[] { 
                existingOffer.AccommodationType,
                existingOffer.Capacity,
                existingOffer.IdOffer
             };
            await _accommodationOfferRepository.DeleteAsync(array);
            return Ok(new { message = "Accommodation offer deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    
    [HttpPost("transportation-offers")]
    public async Task<IActionResult> CreateTransportationOffer([FromBody] TransportationOffer transportationOffer)
    {
        try
        {
            var array = new object[] { transportationOffer.IdRequest, transportationOffer.IdOffer };
            var requestExists = await _offerRepository.ExistsAsync(array);
            if (!requestExists)
                return NotFound(new { error = "Transportation request not found" });

            await _transportationOfferRepository.CreateAsync(transportationOffer);
            return Ok(new { message = "Transportation offer created successfully", data = transportationOffer });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("transportation-offers")]
    public async Task<IActionResult> GetAllTransportationOffers()
    {
        try
        {
            var offers = await _transportationOfferRepository.GetAllAsync();
            return Ok(offers);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("transportation-offers/{idOffer}/{idRequest}")]
    public async Task<IActionResult> UpdateTransportationOffer(int idOffer, int idRequest, [FromBody] TransportationOffer updatedOffer)
    {
        try
        {
            var offers = await _transportationOfferRepository.GetAllAsync();
            var existingOffer = offers.FirstOrDefault(o => o.IdOffer == idOffer && o.IdRequest == idRequest);
            
            if (existingOffer == null)
                return NotFound(new { error = "Transportation offer not found" });
            if (DeleteTransportationOffer(existingOffer.IdOffer, existingOffer.IdRequest) == null)
            {   
                return BadRequest(new { error = "Failed to delete existing offer before update" });
            }

            existingOffer.Type = updatedOffer.Type;
            existingOffer.Capacity = updatedOffer.Capacity;
            existingOffer.CompanyName = updatedOffer.CompanyName;
            existingOffer.EquipmentSpace = updatedOffer.EquipmentSpace;
            existingOffer.AirConditioning = updatedOffer.AirConditioning;
            existingOffer.Tv = updatedOffer.Tv;
            existingOffer.Wifi = updatedOffer.Wifi;
            existingOffer.Restroom = updatedOffer.Restroom;

            await _transportationOfferRepository.UpdateAsync(existingOffer);
            return Ok(new { message = "Transportation offer updated successfully", data = existingOffer });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("transportation-offers/{idOffer}/{idRequest}")]
    public async Task<IActionResult> DeleteTransportationOffer(int idOffer, int idRequest)
    {
        try
        {
            var offers = await _transportationOfferRepository.GetAllAsync();
            var existingOffer = offers.FirstOrDefault(o => o.IdOffer == idOffer && o.IdRequest == idRequest);
            
            if (existingOffer == null)
                return NotFound(new { error = "Transportation offer not found" });

            var array = new object[] { 
                existingOffer.Type,
                existingOffer.Capacity,
                existingOffer.IdOffer
             };
            await _transportationOfferRepository.DeleteAsync(array);
            return Ok(new { message = "Transportation offer deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    
    [HttpGet("analytics/average-price-by-type/{matchId}")]
    public async Task<IActionResult> GetAveragePriceByTypeForMatch(int matchId)
    {
        try
        {
            var query = @"
                SELECT type, AVG(price) AS avg_price
                FROM offer
                WHERE id_match = ?
                GROUP BY type";
            
            var result = await _session.ExecuteAsync(new SimpleStatement(query, matchId));
            var averagePrices = result.Select(row => new {
                Type = row.GetValue<string>("type"),
                AveragePrice = (double)row.GetValue<int>("avg_price")
            }).ToList();

            return Ok(averagePrices);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

   
    [HttpGet("analytics/transportation-requests-by-date/{vehicleType}")]
    public async Task<IActionResult> GetTransportationRequestsCountByDate(string vehicleType)
    {
        try
        {
            var query = @"
                SELECT start_date, COUNT(*) AS total_requests
                FROM transportation_request
                WHERE vehicle_type = ?
                GROUP BY start_date";
            
            var result = await _session.ExecuteAsync(new SimpleStatement(query, vehicleType));
            var requestCounts = result.Select(row => new {
                StartDate = row.GetValue<LocalDate>("start_date").ToString(),
                TotalRequests = row.GetValue<long>("total_requests")
            }).ToList();

            return Ok(requestCounts);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    
    [HttpGet("analytics/min-capacity-by-accommodation-type")]
    public async Task<IActionResult> GetMinCapacityByAccommodationType()
    {
        try
        {
            var query = @"
                SELECT accommodation_type, MIN(capacity) AS min_capacity
                FROM accommodation_offer
                GROUP BY accommodation_type";
            
            var result = await _session.ExecuteAsync(new SimpleStatement(query));
            var minCapacities = result.Select(row => new {
                AccommodationType = row.GetValue<string>("accommodation_type"),
                MinCapacity = row.GetValue<int>("min_capacity")
            }).ToList();

            return Ok(minCapacities);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    
    [HttpGet("analytics/trips-for-match/{matchId}")]
    public async Task<IActionResult> GetTripsForMatch(int matchId)
    {
        try
        {
            var query = @"
                SELECT id_trip, notes, id_accommodation_offer, id_transportation_offer
                FROM trip
                WHERE match_id_match = ?";
            
            var result = await _session.ExecuteAsync(new SimpleStatement(query, matchId));
            var trips = result.Select(row => new {
                IdTrip = row.GetValue<int>("id_trip"),
                Notes = row.GetValue<string>("notes"),
                IdAccommodationOffer = row.GetValue<int?>("id_accommodation_offer"),
                IdTransportationOffer = row.GetValue<int?>("id_transportation_offer")
            }).ToList();

            return Ok(trips);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    
    [HttpGet("analytics/cheapest-transportation-offers/{matchId}/{type}")]
    public async Task<IActionResult> GetCheapestTransportationOffers(int matchId, string type, bool chosen)
    {
        try
        {
            var query = @"
                SELECT id_offer, price, user_id_user
                FROM offer
                WHERE id_match = ? AND type = ? AND chosen = ?
                ORDER BY price ASC
                LIMIT 3";

            var result = await _session.ExecuteAsync(new SimpleStatement(query, matchId, type, chosen));
            var cheapestOffers = result.Select(row => new {
                IdOffer = row.GetValue<int>("id_offer"),
                Price = row.GetValue<int>("price"),
                UserIdUser = row.GetValue<int>("user_id_user")
            }).ToList();

            return Ok(cheapestOffers);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}