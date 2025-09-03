using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;
using travel_service.src.Travels.Core.Application.Features.Visas.GetVisasByTravelInfoId;

namespace travel_service.src.Travels.Core.Application.Features.Visas.GetVisasByTravelInfoId;

public class GetVisasByTravelInfoIdHandler : IRequestHandler<GetVisasByTravelInfoIdQuery, Result<GetVisasByTravelInfoIdResponse>>
{
    private readonly IVisaRepository _visaRepository;

    public GetVisasByTravelInfoIdHandler(IVisaRepository visaRepository)
    {
        _visaRepository = visaRepository;
    }

    public Task<Result<GetVisasByTravelInfoIdResponse>> Handle(GetVisasByTravelInfoIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var visas = _visaRepository.GetByTravelInformationId(request.TravelInfoId);

            var response = new GetVisasByTravelInfoIdResponse
            {
                Visas = visas.Select(v => new VisaDTO
                {
                    VisaNumber = v.VisaNumber,
                    State = v.State,
                    CreationDate = v.CreationDate,
                    ExpirationDate = v.ExpirationDate,
                    IdTravelInformation = v.IdTravelInformation
                }).ToList()
            };

            return Task.FromResult(Result<GetVisasByTravelInfoIdResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetVisasByTravelInfoIdResponse>.Failure($"An error occurred while retrieving visas: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}