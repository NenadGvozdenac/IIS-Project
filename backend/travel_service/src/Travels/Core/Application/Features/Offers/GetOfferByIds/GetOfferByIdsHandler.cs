using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Application.Features.Offers.GetAllOffers;

namespace travel_service.src.Travels.Core.Application.Features.Offers.GetOfferByIds;

public class GetOfferByIdsHandler : IRequestHandler<GetOfferByIdsQuery, Result<GetOfferByIdsResponse>>
{
    private readonly IOffersRepository _offersRepository;

    public GetOfferByIdsHandler(IOffersRepository offersRepository)
    {
        _offersRepository = offersRepository;
    }

    public Task<Result<GetOfferByIdsResponse>> Handle(GetOfferByIdsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var chosenOffer = _offersRepository.GetChosenOfferByTypeAndMatch(request.Type, request.IdMatch);

            if (chosenOffer == null)
            {
                var response = new GetOfferByIdsResponse
                {
                    ChosenOffer = null,
                    Type = request.Type,
                    IdMatch = request.IdMatch,
                    HasChosenOffer = false,
                    Message = $"No chosen {request.Type} offer found for match {request.IdMatch}"
                };

                return Task.FromResult(Result<GetOfferByIdsResponse>.Success(response));
            }

            var offerDto = new OfferDto
            {
                IdOffer = chosenOffer.IdOffer,
                Price = chosenOffer.Price,
                UserIdUser = chosenOffer.UserIdUser,
                IdMatch = chosenOffer.IdMatch,
                IdAgency = chosenOffer.IdAgency,
                IdRequest = chosenOffer.IdRequest,
                Type = chosenOffer.Type,
                Chosen = chosenOffer.Chosen,
                
                // Accommodation properties
                Name = chosenOffer.AccommodationOffer?.Name,
                Capacity = chosenOffer.AccommodationOffer?.Capacity ?? chosenOffer.TransportationOffer?.Capacity,
                AccommodationType = chosenOffer.AccommodationOffer?.AccommodationType,
                DoubleRoom = chosenOffer.AccommodationOffer?.DoubleRoom,
                TripleRoom = chosenOffer.AccommodationOffer?.TripleRoom,
                QuadrupleRoom = chosenOffer.AccommodationOffer?.QuadrupleRoom,
                Breakfast = chosenOffer.AccommodationOffer?.Breakfast,
                FitnessCenter = chosenOffer.AccommodationOffer?.FitnessCenter,
                Pool = chosenOffer.AccommodationOffer?.Pool,
                Wifi = chosenOffer.AccommodationOffer?.Wifi,
                Spa = chosenOffer.AccommodationOffer?.Spa,
                
                // Transportation properties
                CompanyName = chosenOffer.TransportationOffer?.CompanyName,
                VehicleType = chosenOffer.TransportationOffer?.Type,
                EquipmentSpace = chosenOffer.TransportationOffer?.EquipmentSpace,
                AirConditioning = chosenOffer.TransportationOffer?.AirConditioning,
                Tv = chosenOffer.TransportationOffer?.Tv,
                WifiTransport = chosenOffer.TransportationOffer?.Wifi,
                Restroom = chosenOffer.TransportationOffer?.Restroom
            };

            var successResponse = new GetOfferByIdsResponse
            {
                ChosenOffer = offerDto,
                Type = request.Type,
                IdMatch = request.IdMatch,
                HasChosenOffer = true,
                Message = $"Chosen {request.Type} offer found successfully"
            };

            return Task.FromResult(Result<GetOfferByIdsResponse>.Success(successResponse));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetOfferByIdsResponse>.Failure($"An error occurred while retrieving the chosen offer: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
