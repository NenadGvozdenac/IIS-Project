using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.Seasons.GetSeasonById;

public class GetSeasonByIdHandler : IRequestHandler<GetSeasonByIdQuery, Result<GetSeasonByIdResponse>>
{
    private readonly ISeasonRepository _seasonRepository;

    public GetSeasonByIdHandler(ISeasonRepository seasonRepository)
    {
        _seasonRepository = seasonRepository;
    }

    public Task<Result<GetSeasonByIdResponse>> Handle(GetSeasonByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var season = _seasonRepository.GetById(request.Id);

            if (season == null)
            {
                return Task.FromResult(Result<GetSeasonByIdResponse>.Failure($"Season with ID {request.Id} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var response = new GetSeasonByIdResponse
            {
                IdSeason = season.IdSeason,
                Name = season.Name,
                StartedAt = season.StartedAt,
                EndedAt = season.EndedAt
            };

            return Task.FromResult(Result<GetSeasonByIdResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetSeasonByIdResponse>.Failure($"An error occurred while retrieving the season: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
