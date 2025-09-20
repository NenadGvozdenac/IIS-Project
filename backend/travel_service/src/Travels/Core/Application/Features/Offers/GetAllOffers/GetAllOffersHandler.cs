using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.Offers.GetAllOffers;

public class GetAllOffersHandler : IRequestHandler<GetAllOffersQuery, Result<GetAllOffersResponse>>
{
    private readonly IOffersRepository _offersRepository;

    public GetAllOffersHandler(IOffersRepository offersRepository)
    {
        _offersRepository = offersRepository;
    }

    public Task<Result<GetAllOffersResponse>> Handle(GetAllOffersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var offers = _offersRepository.GetOffersByTypeAndMatch(request.Type, request.IdMatch);

            var offerDtos = offers.Select(offer => new OfferDto
            {
                IdOffer = offer.IdOffer,
                Price = offer.Price,
                UserIdUser = offer.UserIdUser,
                IdMatch = offer.IdMatch,
                IdAgency = offer.IdAgency,
                IdRequest = offer.IdRequest,
                Type = offer.Type,
                Chosen = offer.Chosen,
                
                // Accommodation properties
                Name = offer.AccommodationOffer?.Name,
                Capacity = offer.AccommodationOffer?.Capacity ?? offer.TransportationOffer?.Capacity,
                AccommodationType = offer.AccommodationOffer?.AccommodationType,
                DoubleRoom = offer.AccommodationOffer?.DoubleRoom,
                TripleRoom = offer.AccommodationOffer?.TripleRoom,
                QuadrupleRoom = offer.AccommodationOffer?.QuadrupleRoom,
                Breakfast = offer.AccommodationOffer?.Breakfast,
                FitnessCenter = offer.AccommodationOffer?.FitnessCenter,
                Pool = offer.AccommodationOffer?.Pool,
                Wifi = offer.AccommodationOffer?.Wifi,
                Spa = offer.AccommodationOffer?.Spa,
                
                // Transportation properties
                CompanyName = offer.TransportationOffer?.CompanyName,
                VehicleType = offer.TransportationOffer?.Type,
                EquipmentSpace = offer.TransportationOffer?.EquipmentSpace,
                AirConditioning = offer.TransportationOffer?.AirConditioning,
                Tv = offer.TransportationOffer?.Tv,
                WifiTransport = offer.TransportationOffer?.Wifi,
                Restroom = offer.TransportationOffer?.Restroom
            }).ToList();

            var response = new GetAllOffersResponse
            {
                Offers = offerDtos,
                Type = request.Type,
                IdMatch = request.IdMatch,
                TotalCount = offerDtos.Count
            };

            return Task.FromResult(Result<GetAllOffersResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetAllOffersResponse>.Failure($"An error occurred while retrieving offers: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
