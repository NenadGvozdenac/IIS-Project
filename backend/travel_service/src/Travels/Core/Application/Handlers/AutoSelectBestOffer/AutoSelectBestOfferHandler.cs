using MediatR;
using travel_service.src.Travels.Core.Application.Commands.AutoSelectBestOffer;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Handlers.AutoSelectBestOffer;

public class AutoSelectBestOfferHandler : IRequestHandler<AutoSelectBestOfferCommand, Result<AutoSelectBestOfferResponse>>
{
    private readonly IOffersRepository _offersRepository;

    public AutoSelectBestOfferHandler(IOffersRepository offersRepository)
    {
        _offersRepository = offersRepository;
    }

    public async Task<Result<AutoSelectBestOfferResponse>> Handle(AutoSelectBestOfferCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate offer type
            if (request.OfferType != "transportation" && request.OfferType != "accommodation")
            {
                return Result<AutoSelectBestOfferResponse>.Failure("Invalid offer type. Must be 'transportation' or 'accommodation'.")
                    .WithCode((int)ResultCode.BadRequest);
            }

            // Validate weights sum to 1.0 (or close to it)
            var totalWeight = request.WeightPrice + request.WeightCapacity + request.WeightBenefits + request.WeightAgency;
            if (Math.Abs(totalWeight - 1.0m) > 0.01m)
            {
                return Result<AutoSelectBestOfferResponse>.Failure("Weights must sum to approximately 1.0")
                    .WithCode((int)ResultCode.BadRequest);
            }

            // Call repository method
            var result = await _offersRepository.AutoSelectBestOffer(
                request.MatchId, 
                request.OfferType, 
                request.WeightPrice, 
                request.WeightCapacity, 
                request.WeightBenefits, 
                request.WeightAgency);

            if (result == null)
            {
                return Result<AutoSelectBestOfferResponse>.Failure("No offers found or auto-selection failed")
                    .WithCode((int)ResultCode.NotFound);
            }

            return Result<AutoSelectBestOfferResponse>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<AutoSelectBestOfferResponse>.Failure($"Error during auto-selection: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}