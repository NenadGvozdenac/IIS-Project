using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.Agencies.GetAllAgencies;

public class GetAllAgenciesHandler : IRequestHandler<GetAllAgenciesQuery, Result<GetAllAgenciesResponse>>
{
    private readonly IAgenciesRepository _agenciesRepository;

    public GetAllAgenciesHandler(IAgenciesRepository agenciesRepository)
    {
        _agenciesRepository = agenciesRepository;
    }

    public Task<Result<GetAllAgenciesResponse>> Handle(GetAllAgenciesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var agencies = _agenciesRepository.GetByType(request.Type);
            var response = new GetAllAgenciesResponse
            {
                Agencies = agencies.Select(a => new AgencyDto
                {
                    IdAgency = a.IdAgency,
                    Name = a.Name,
                    Email = a.Email,
                    Type = a.Type
                }).ToList()
            };

            return Task.FromResult(Result<GetAllAgenciesResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetAllAgenciesResponse>.Failure($"An error occurred while retrieving agencies: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
