using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;
using scouting_service.src.Scoutings.Core.Domain.Entities;

namespace scouting_service.src.Scoutings.Core.Application.Features.Nationalities.CreateNationality;

public class CreateNationalityHandler : IRequestHandler<CreateNationalityCommand, Result<CreateNationalityResponse>>
{
    private readonly INationalityRepository _nationalityRepository;

    public CreateNationalityHandler(INationalityRepository nationalityRepository)
    {
        _nationalityRepository = nationalityRepository;
    }

    public Task<Result<CreateNationalityResponse>> Handle(CreateNationalityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var nationality = new Nationality
            {
                State = request.State
            };

            var createdNationality = _nationalityRepository.Create(nationality);

            var response = new CreateNationalityResponse
            {
                IdNationality = createdNationality.IdNationality,
                State = createdNationality.State
            };

            return Task.FromResult(Result<CreateNationalityResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreateNationalityResponse>.Failure($"An error occurred while creating nationality: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
