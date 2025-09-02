using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Seasons.DeleteSeason;

public class DeleteSeasonHandler : IRequestHandler<DeleteSeasonCommand, Result<DeleteSeasonResponse>>
{
    private readonly ISeasonRepository _seasonRepository;

    public DeleteSeasonHandler(ISeasonRepository seasonRepository)
    {
        _seasonRepository = seasonRepository;
    }

    public Task<Result<DeleteSeasonResponse>> Handle(DeleteSeasonCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var season = _seasonRepository.GetById(request.IdSeason);
            if (season == null)
            {
                return Task.FromResult(Result<DeleteSeasonResponse>.Failure("Season not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Check if season has associated matches or season tickets
            if (season.Matches.Any())
            {
                return Task.FromResult(Result<DeleteSeasonResponse>.Failure("Cannot delete season that has associated matches")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (season.SeasonTickets.Any())
            {
                return Task.FromResult(Result<DeleteSeasonResponse>.Failure("Cannot delete season that has associated season tickets")
                    .WithCode((int)ResultCode.BadRequest));
            }

            var deleted = _seasonRepository.Delete(request.IdSeason);
            if (!deleted)
            {
                return Task.FromResult(Result<DeleteSeasonResponse>.Failure("Failed to delete season")
                    .WithCode((int)ResultCode.InternalServerError));
            }

            var response = new DeleteSeasonResponse
            {
                Message = "Season deleted successfully"
            };

            return Task.FromResult(Result<DeleteSeasonResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<DeleteSeasonResponse>.Failure($"An error occurred while deleting the season: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
