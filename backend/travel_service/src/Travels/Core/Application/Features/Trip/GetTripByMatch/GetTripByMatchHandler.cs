using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.Trip.GetTripByMatch;

public class GetTripByMatchHandler : IRequestHandler<GetTripByMatchQuery, Result<GetTripByMatchResponse>>
{
    private readonly ITripRepository _tripRepository;

    public GetTripByMatchHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public Task<Result<GetTripByMatchResponse>> Handle(GetTripByMatchQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Proverava da li match postoji
            if (!_tripRepository.DoesMatchExist(request.MatchId))
            {
                return Task.FromResult(Result<GetTripByMatchResponse>.Failure("Match not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Traži putovanje za dati match
            var trip = _tripRepository.GetTripByMatch(request.MatchId);

            var response = new GetTripByMatchResponse
            {
                HasTrip = trip != null,
                Trip = trip != null ? new TripDto
                {
                    IdTrip = trip.IdTrip,
                    Notes = trip.Notes,
                    MatchIdMatch = trip.MatchIdMatch,
                    IdAccommodationOffer = trip.IdAccommodationOffer,
                    IdTransportationOffer = trip.IdTransportationOffer,
                    IdAccommodationAgency = trip.IdAccommodationAgency,
                    IdTransportationAgency = trip.IdTransportationAgency,
                    IdAccommodationRequest = trip.IdAccommodationRequest,
                    IdTransportationRequest = trip.IdTransportationRequest
                } : null
            };

            return Task.FromResult(Result<GetTripByMatchResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetTripByMatchResponse>.Failure($"An error occurred while retrieving the trip: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}