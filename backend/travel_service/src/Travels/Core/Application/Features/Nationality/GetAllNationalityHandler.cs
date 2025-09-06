using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.Nationality.GetAllNationality;

public class GetAllNationalityHandler : IRequestHandler<GetAllNationalityQuery, Result<List<GetAllNationalityResponse>>>
{
    private readonly INationalityRepository _nationalityRepository;

    public GetAllNationalityHandler(INationalityRepository nationalityRepository)
    {
        _nationalityRepository = nationalityRepository;
    }

    public Task<Result<List<GetAllNationalityResponse>>> Handle(GetAllNationalityQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var nationalities = _nationalityRepository.GetAll();
            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            var response = nationalities.Select(n => new GetAllNationalityResponse
            {
                IdNationality = n.IdNationality,
                State = n.State,
            }).ToList();

            return Task.FromResult(Result<List<GetAllNationalityResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetAllNationalityResponse>>.Failure($"An error occurred while retrieving nationalities: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
