using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Domain.Entities;

namespace travel_service.src.Travels.Core.Application.Features.Offers.CreateOffer;

public class CreateOfferHandler : IRequestHandler<CreateOfferCommand, Result<CreateOfferResponse>>
{
    private readonly IOffersRepository _offersRepository;
    private readonly IUserRepository _userRepository;

    public CreateOfferHandler(IOffersRepository offersRepository, IUserRepository userRepository)
    {
        _offersRepository = offersRepository;
        _userRepository = userRepository;
    }

    public Task<Result<CreateOfferResponse>> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validacija korisnika
            var user = _userRepository.GetById(request.UserId);
            if (user == null)
            {
                return Task.FromResult(Result<CreateOfferResponse>.Failure("User not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            if (user.Type != "team manager")
            {
                return Task.FromResult(Result<CreateOfferResponse>.Failure("Only agency can create offers")
                    .WithCode((int)ResultCode.Forbidden));
            }

            // Validacija osnovnih polja
            if (string.IsNullOrWhiteSpace(request.Type) || 
                (request.Type != "accommodation" && request.Type != "transportation"))
            {
                return Task.FromResult(Result<CreateOfferResponse>.Failure("Type must be 'accommodation' or 'transportation'")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Kreiranje osnovnog Offer-a
            var newOffer = new Offer
            {
                Price = request.Price,
                UserIdUser = request.UserId,
                IdMatch = request.IdMatch,
                IdAgency = request.IdAgency,
                IdRequest = request.IdRequest,
                Type = request.Type,
                Chosen = false
            };

            // Kreiranje specifičnog tipa ponude
            if (request.Type == "accommodation")
            {
                if (string.IsNullOrWhiteSpace(request.Name) || request.Capacity == null)
                {
                    return Task.FromResult(Result<CreateOfferResponse>.Failure("Name and Capacity are required for accommodation offers")
                        .WithCode((int)ResultCode.BadRequest));
                }

                newOffer.AccommodationOffer = new AccommodationOffer
                {
                    Name = request.Name,
                    Capacity = request.Capacity,
                    AccommodationType = request.AccommodationType,
                    IdAgency = request.IdAgency,
                    IdRequest = request.IdRequest,
                    DoubleRoom = request.DoubleRoom,
                    TripleRoom = request.TripleRoom,
                    QuadrupleRoom = request.QuadrupleRoom,
                    Breakfast = request.Breakfast,
                    FitnessCenter = request.FitnessCenter,
                    Pool = request.Pool,
                    Wifi = request.Wifi,
                    Spa = request.Spa
                };
            }
            else if (request.Type == "transportation")
            {
                if (string.IsNullOrWhiteSpace(request.CompanyName) || request.Capacity == null)
                {
                    return Task.FromResult(Result<CreateOfferResponse>.Failure("CompanyName and Capacity are required for transportation offers")
                        .WithCode((int)ResultCode.BadRequest));
                }

                newOffer.TransportationOffer = new TransportationOffer
                {
                    CompanyName = request.CompanyName,
                    Capacity = request.Capacity,
                    Type = request.VehicleType,
                    IdAgency = request.IdAgency,
                    IdRequest = request.IdRequest,
                    EquipmentSpace = request.EquipmentSpace,
                    AirConditioning = request.AirConditioning,
                    Tv = request.Tv,
                    Wifi = request.WifiTransport,
                    Restroom = request.Restroom
                };
            }

            // Čuvanje Offer-a
            var createdOffer = _offersRepository.CreateOffer(newOffer);

            var response = new CreateOfferResponse
            {
                IdOffer = createdOffer.IdOffer,
                Type = createdOffer.Type ?? string.Empty,
                Price = createdOffer.Price,
                IdMatch = createdOffer.IdMatch,
                IdAgency = createdOffer.IdAgency,
                IdRequest = createdOffer.IdRequest,
                UserId = request.UserId
            };

            return Task.FromResult(Result<CreateOfferResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreateOfferResponse>.Failure($"An error occurred while creating the offer: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
