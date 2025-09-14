using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Seasons.GetSeasonById;

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
            var season = _seasonRepository.GetById(request.IdSeason);

            if (season == null)
            {
                return Task.FromResult(Result<GetSeasonByIdResponse>.Failure("Season not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            var response = new GetSeasonByIdResponse
            {
                IdSeason = season.IdSeason,
                StartedAt = season.StartedAt,
                EndedAt = season.EndedAt,
                Name = season.Name,
                MatchesCount = season.Matches.Count,
                SeasonTicketsCount = season.SeasonTickets.Count,
                IsActive = season.StartedAt <= currentDate && (!season.EndedAt.HasValue || season.EndedAt >= currentDate),
                TicketsForSale = season.TicketsForSale,
                TicketsWentOnSale = season.TicketsWentOnSale,
                Matches = season.Matches.Select(m => new MatchResponse
                {
                    IdMatch = m.IdMatch,
                    Name = m.Name,
                    City = m.City,
                    Hall = m.Hall,
                    ScheduledAt = m.ScheduledAt
                }).ToList(),
                SeasonTickets = season.SeasonTickets.Select(st => new SeasonTicketResponse
                {
                    IdPurchaseOffer = st.IdPurchaseOffer,
                    TicketPrice = st.TicketPrice
                }).ToList()
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
