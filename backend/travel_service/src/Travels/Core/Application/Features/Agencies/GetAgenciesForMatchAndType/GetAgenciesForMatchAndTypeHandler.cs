using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.Agencies.GetAgenciesForMatchAndType;

public class GetAgenciesForMatchAndTypeHandler : IRequestHandler<GetAgenciesForMatchAndTypeQuery, Result<GetAgenciesForMatchAndTypeResponse>>
{
    private readonly IAgenciesRepository _agenciesRepository;

    public GetAgenciesForMatchAndTypeHandler(IAgenciesRepository agenciesRepository)
    {
        _agenciesRepository = agenciesRepository;
    }

    public Task<Result<GetAgenciesForMatchAndTypeResponse>> Handle(GetAgenciesForMatchAndTypeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var agencies = _agenciesRepository.GetAgenciesForMatchAndType(request.MatchId, request.Type);
            
            var response = new GetAgenciesForMatchAndTypeResponse
            {
                Agencies = agencies
            };

            return Task.FromResult(Result<GetAgenciesForMatchAndTypeResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetAgenciesForMatchAndTypeResponse>.Failure($"An error occurred while getting agencies: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}