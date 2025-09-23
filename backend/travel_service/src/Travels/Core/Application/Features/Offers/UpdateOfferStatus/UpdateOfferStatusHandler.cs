using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.Offers.UpdateOfferStatus;

public class UpdateOfferStatusHandler : IRequestHandler<UpdateOfferStatusCommand, Result<UpdateOfferStatusResponse>>
{
    private readonly IOffersRepository _offersRepository;

    public UpdateOfferStatusHandler(IOffersRepository offersRepository)
    {
        _offersRepository = offersRepository;
    }

    public Task<Result<UpdateOfferStatusResponse>> Handle(UpdateOfferStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingOffer = _offersRepository.GetOfferByCompositeKey(
                request.IdOffer, request.IdAgency, request.IdRequest);
            if (existingOffer == null)
            {
                return Task.FromResult(Result<UpdateOfferStatusResponse>.Failure("Offer not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            _offersRepository.UpdateOfferStatus(
                request.IdOffer, 
                request.IdAgency, 
                request.IdRequest, 
                request.Chosen);

            var response = new UpdateOfferStatusResponse
            {
                IdOffer = request.IdOffer,
                IdAgency = request.IdAgency,
                IdRequest = request.IdRequest,
                Chosen = request.Chosen,
                Type = request.Type,
                IdMatch = request.IdMatch
            };

            return Task.FromResult(Result<UpdateOfferStatusResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<UpdateOfferStatusResponse>.Failure($"An error occurred while updating offer status: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
