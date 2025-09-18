using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Competitions.GetActiveCompetitions;

public class GetActiveCompetitionsHandler : IRequestHandler<GetActiveCompetitionsQuery, Result<IEnumerable<GetActiveCompetitionsResponse>>>
{
    private readonly ICompetitionRepository _competitionRepository;

    public GetActiveCompetitionsHandler(ICompetitionRepository competitionRepository)
    {
        _competitionRepository = competitionRepository;
    }

    public Task<Result<IEnumerable<GetActiveCompetitionsResponse>>> Handle(GetActiveCompetitionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var activeCompetitions = _competitionRepository.GetActiveCompetitions();

            var response = activeCompetitions.Select(c => new GetActiveCompetitionsResponse
            {
                IdCompetition = c.IdCompetition,
                Name = c.Name,
                StartedAt = c.StartedAt,
                EndedAt = c.EndedAt,
                NumberOfMatches = c.NumberOfMatches,
                CurrentMatches = c.Matches.Count()
            });

            return Task.FromResult(Result<IEnumerable<GetActiveCompetitionsResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<IEnumerable<GetActiveCompetitionsResponse>>.Failure($"An error occurred while retrieving active competitions: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
