using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Competitions.GetAllCompetitions;

public class GetAllCompetitionsHandler : IRequestHandler<GetAllCompetitionsQuery, Result<IEnumerable<GetAllCompetitionsResponse>>>
{
    private readonly ICompetitionRepository _competitionRepository;

    public GetAllCompetitionsHandler(ICompetitionRepository competitionRepository)
    {
        _competitionRepository = competitionRepository;
    }

    public Task<Result<IEnumerable<GetAllCompetitionsResponse>>> Handle(GetAllCompetitionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var competitions = _competitionRepository.GetAll();
            var today = DateOnly.FromDateTime(DateTime.Today);

            var response = competitions.Select(c => new GetAllCompetitionsResponse
            {
                IdCompetition = c.IdCompetition,
                Name = c.Name,
                StartedAt = c.StartedAt,
                EndedAt = c.EndedAt,
                NumberOfMatches = c.NumberOfMatches,
                IsActive = c.StartedAt <= today && (c.EndedAt == null || c.EndedAt >= today),
                Matches = c.Matches.Select(m => new MatchInfo
                {
                    IdMatch = m.IdMatch,
                    Name = m.Name,
                    Date = m.ScheduledAt
                }).ToList()
            });

            return Task.FromResult(Result<IEnumerable<GetAllCompetitionsResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<IEnumerable<GetAllCompetitionsResponse>>.Failure($"An error occurred while retrieving competitions: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
