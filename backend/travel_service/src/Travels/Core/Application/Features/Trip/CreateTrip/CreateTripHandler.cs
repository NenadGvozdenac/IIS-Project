using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Features.Trip.CreateTrip;

public class CreateTripHandler : IRequestHandler<CreateTripCommand, Result<CreateTripResponse>>
{
    private readonly ITripRepository _tripRepository;

    public CreateTripHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public Task<Result<CreateTripResponse>> Handle(CreateTripCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validacija - proverava da li match postoji
            if (!_tripRepository.DoesMatchExist(request.MatchIdMatch))
            {
                return Task.FromResult(Result<CreateTripResponse>.Failure("Match not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Validacija - proverava da li već postoji putovanje za taj match
            if (_tripRepository.DoesTripExistForMatch(request.MatchIdMatch))
            {
                return Task.FromResult(Result<CreateTripResponse>.Failure("Trip already exists for this match")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Validacija - proverava da li transportation offer postoji
            if (!_tripRepository.DoesTransportationOfferExist(
                request.IdTransportationOffer, 
                request.IdTransportationAgency, 
                request.IdTransportationRequest))
            {
                return Task.FromResult(Result<CreateTripResponse>.Failure("Transportation offer not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Validacija - proverava da li je transportation offer chosen
            if (!_tripRepository.IsTransportationOfferChosen(
                request.IdTransportationOffer, 
                request.IdTransportationAgency, 
                request.IdTransportationRequest))
            {
                return Task.FromResult(Result<CreateTripResponse>.Failure("Transportation offer must be chosen to create a trip")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Validacija za accommodation offer (samo ako je prosleđen)
            if (request.IdAccommodationOffer.HasValue || 
                request.IdAccommodationAgency.HasValue || 
                request.IdAccommodationRequest.HasValue)
            {
                // Ako je bilo koji accommodation podatak prosleđen, svi moraju biti prosleđeni
                if (!request.IdAccommodationOffer.HasValue || 
                    !request.IdAccommodationAgency.HasValue || 
                    !request.IdAccommodationRequest.HasValue)
                {
                    return Task.FromResult(Result<CreateTripResponse>.Failure("If any accommodation data is provided, all accommodation fields (IdAccommodationOffer, IdAccommodationAgency, IdAccommodationRequest) must be provided")
                        .WithCode((int)ResultCode.BadRequest));
                }

                if (!_tripRepository.DoesAccommodationOfferExist(
                    request.IdAccommodationOffer.Value, 
                    request.IdAccommodationAgency.Value, 
                    request.IdAccommodationRequest.Value))
                {
                    return Task.FromResult(Result<CreateTripResponse>.Failure("Accommodation offer not found")
                        .WithCode((int)ResultCode.NotFound));
                }

                if (!_tripRepository.IsAccommodationOfferChosen(
                    request.IdAccommodationOffer.Value, 
                    request.IdAccommodationAgency.Value, 
                    request.IdAccommodationRequest.Value))
                {
                    return Task.FromResult(Result<CreateTripResponse>.Failure("Accommodation offer must be chosen to create a trip")
                        .WithCode((int)ResultCode.BadRequest));
                }
            }

            // Kreiranje trip entiteta
            var trip = new Domain.Entities.Trip
            {
                Notes = request.Notes,
                MatchIdMatch = request.MatchIdMatch,
                IdTransportationOffer = request.IdTransportationOffer,
                IdTransportationAgency = request.IdTransportationAgency,
                IdTransportationRequest = request.IdTransportationRequest,
                IdAccommodationOffer = request.IdAccommodationOffer,
                IdAccommodationAgency = request.IdAccommodationAgency,
                IdAccommodationRequest = request.IdAccommodationRequest
            };

            // Kreiranje putovanja u bazi
            var createdTrip = _tripRepository.CreateTrip(trip);

            var response = new CreateTripResponse
            {
                IdTrip = createdTrip.IdTrip,
                Notes = createdTrip.Notes,
                MatchIdMatch = createdTrip.MatchIdMatch,
                IdTransportationOffer = request.IdTransportationOffer,
                IdTransportationAgency = request.IdTransportationAgency,
                IdTransportationRequest = request.IdTransportationRequest,
                IdAccommodationOffer = createdTrip.IdAccommodationOffer,
                IdAccommodationAgency = createdTrip.IdAccommodationAgency,
                IdAccommodationRequest = createdTrip.IdAccommodationRequest,
                Message = "Trip created successfully"
            };

            return Task.FromResult(Result<CreateTripResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreateTripResponse>.Failure($"An error occurred while creating trip: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
