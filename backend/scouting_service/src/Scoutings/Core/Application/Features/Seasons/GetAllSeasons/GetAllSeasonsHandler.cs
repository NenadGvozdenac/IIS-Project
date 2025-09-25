using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.Seasons.GetAllSeasons;

public class GetAllSeasonsHandler : IRequestHandler<GetAllSeasonsQuery, Result<GetAllSeasonsResponse>>
{
    private readonly ISeasonRepository _seasonRepository;

    public GetAllSeasonsHandler(ISeasonRepository seasonRepository)
    {
        _seasonRepository = seasonRepository;
    }

    public Task<Result<GetAllSeasonsResponse>> Handle(GetAllSeasonsQuery request, CancellationToken cancellationToken)
    {
        var seasons = _seasonRepository.GetAll();

        var seasonDtos = seasons.Select(s => new SeasonDto
        {
            Id = s.IdSeason,
            Name = s.Name ?? string.Empty,
            StartedAt = s.StartedAt,
            EndedAt = s.EndedAt
        }).ToList();

        var response = new GetAllSeasonsResponse
        {
            Seasons = seasonDtos
        };

        return Task.FromResult(Result<GetAllSeasonsResponse>.Success(response));
    }
}
