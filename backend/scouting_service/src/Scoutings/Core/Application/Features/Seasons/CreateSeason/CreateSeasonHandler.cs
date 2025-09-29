using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Domain.Entities;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.Seasons.CreateSeason;

public class CreateSeasonHandler : IRequestHandler<CreateSeasonCommand, Result<CreateSeasonResponse>>
{
    private readonly ISeasonRepository _seasonRepository;

    public CreateSeasonHandler(ISeasonRepository seasonRepository)
    {
        _seasonRepository = seasonRepository;
    }

    public Task<Result<CreateSeasonResponse>> Handle(CreateSeasonCommand request, CancellationToken cancellationToken)
    {
        var season = new Season
        {
            Name = request.Name,
            StartedAt = request.StartedAt,
            EndedAt = request.EndedAt
        };

        var createdSeason = _seasonRepository.Create(season);

        if (createdSeason == null)
        {
            return Task.FromResult(Result<CreateSeasonResponse>.Failure("Failed to create season"));
        }

        var response = new CreateSeasonResponse
        {
            Id = createdSeason.IdSeason,
            Name = createdSeason.Name,
            StartedAt = createdSeason.StartedAt,
            EndedAt = createdSeason.EndedAt
        };

        return Task.FromResult(Result<CreateSeasonResponse>.Success(response));
    }
}
