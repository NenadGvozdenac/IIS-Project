using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.Nationalities.GetAllNationalities;

public class GetAllNationalitiesHandler : IRequestHandler<GetAllNationalitiesQuery, Result<List<GetAllNationalitiesResponse>>>
{
    private readonly INationalityRepository _nationalityRepository;

    public GetAllNationalitiesHandler(INationalityRepository nationalityRepository)
    {
        _nationalityRepository = nationalityRepository;
    }

    public Task<Result<List<GetAllNationalitiesResponse>>> Handle(GetAllNationalitiesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var nationalities = _nationalityRepository.GetAll();

            var response = nationalities.Select(n => new GetAllNationalitiesResponse
            {
                IdNationality = n.IdNationality,
                State = n.State
            }).ToList();

            return Task.FromResult(Result<List<GetAllNationalitiesResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetAllNationalitiesResponse>>.Failure($"An error occurred while retrieving nationalities: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
