using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Seasons.GetAllSeasons;

public class GetAllSeasonsHandler : IRequestHandler<GetAllSeasonsQuery, Result<List<GetAllSeasonsResponse>>>
{
    private readonly ISeasonRepository _seasonRepository;

    public GetAllSeasonsHandler(ISeasonRepository seasonRepository)
    {
        _seasonRepository = seasonRepository;
    }

    public Task<Result<List<GetAllSeasonsResponse>>> Handle(GetAllSeasonsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var seasons = _seasonRepository.GetAll();
            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            var response = seasons.Select(s => new GetAllSeasonsResponse
            {
                IdSeason = s.IdSeason,
                StartedAt = s.StartedAt,
                EndedAt = s.EndedAt,
                Name = s.Name,
                MatchesCount = s.Matches.Count,
                SeasonTicketsCount = s.SeasonTickets.Count,
                IsActive = s.StartedAt <= currentDate && (!s.EndedAt.HasValue || s.EndedAt >= currentDate)
            }).ToList();

            return Task.FromResult(Result<List<GetAllSeasonsResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetAllSeasonsResponse>>.Failure($"An error occurred while retrieving seasons: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
