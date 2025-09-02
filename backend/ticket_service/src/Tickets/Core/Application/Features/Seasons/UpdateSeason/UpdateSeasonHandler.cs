using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Seasons.UpdateSeason;

public class UpdateSeasonHandler : IRequestHandler<UpdateSeasonCommand, Result<UpdateSeasonResponse>>
{
    private readonly ISeasonRepository _seasonRepository;

    public UpdateSeasonHandler(ISeasonRepository seasonRepository)
    {
        _seasonRepository = seasonRepository;
    }

    public Task<Result<UpdateSeasonResponse>> Handle(UpdateSeasonCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingSeason = _seasonRepository.GetById(request.IdSeason);
            if (existingSeason == null)
            {
                return Task.FromResult(Result<UpdateSeasonResponse>.Failure("Season not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Validate that season name is unique (excluding current season)
            if (_seasonRepository.ExistsByName(request.Name, request.IdSeason))
            {
                return Task.FromResult(Result<UpdateSeasonResponse>.Failure("A season with this name already exists")
                    .WithCode((int)ResultCode.Conflict));
            }

            // Validate dates
            if (request.EndedAt.HasValue && request.EndedAt <= request.StartedAt)
            {
                return Task.FromResult(Result<UpdateSeasonResponse>.Failure("End date must be after start date")
                    .WithCode((int)ResultCode.BadRequest));
            }

            existingSeason.StartedAt = request.StartedAt;
            existingSeason.EndedAt = request.EndedAt;
            existingSeason.Name = request.Name;

            var updatedSeason = _seasonRepository.Update(existingSeason);

            var response = new UpdateSeasonResponse
            {
                IdSeason = updatedSeason.IdSeason,
                StartedAt = updatedSeason.StartedAt,
                EndedAt = updatedSeason.EndedAt,
                Name = updatedSeason.Name,
                Message = "Season updated successfully"
            };

            return Task.FromResult(Result<UpdateSeasonResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<UpdateSeasonResponse>.Failure($"An error occurred while updating the season: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
