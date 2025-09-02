using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;
using ticket_service.src.Tickets.Core.Domain.Entities;

namespace ticket_service.src.Tickets.Core.Application.Features.Seasons.CreateSeason;

public class CreateSeasonHandler : IRequestHandler<CreateSeasonCommand, Result<CreateSeasonResponse>>
{
    private readonly ISeasonRepository _seasonRepository;

    public CreateSeasonHandler(ISeasonRepository seasonRepository)
    {
        _seasonRepository = seasonRepository;
    }

    public Task<Result<CreateSeasonResponse>> Handle(CreateSeasonCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate that season name is unique
            if (_seasonRepository.ExistsByName(request.Name))
            {
                return Task.FromResult(Result<CreateSeasonResponse>.Failure("A season with this name already exists")
                    .WithCode((int)ResultCode.Conflict));
            }

            // Validate dates
            if (request.EndedAt.HasValue && request.EndedAt <= request.StartedAt)
            {
                return Task.FromResult(Result<CreateSeasonResponse>.Failure("End date must be after start date")
                    .WithCode((int)ResultCode.BadRequest));
            }

            var season = new Season
            {
                StartedAt = request.StartedAt,
                EndedAt = request.EndedAt,
                Name = request.Name
            };

            var createdSeason = _seasonRepository.Create(season);

            var response = new CreateSeasonResponse
            {
                IdSeason = createdSeason.IdSeason,
                StartedAt = createdSeason.StartedAt,
                EndedAt = createdSeason.EndedAt,
                Name = createdSeason.Name,
                Message = "Season created successfully"
            };

            return Task.FromResult(Result<CreateSeasonResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreateSeasonResponse>.Failure($"An error occurred while creating the season: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
