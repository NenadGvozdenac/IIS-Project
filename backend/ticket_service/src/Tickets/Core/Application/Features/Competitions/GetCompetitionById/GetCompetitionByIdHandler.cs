using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Competitions.GetCompetitionById;

public class GetCompetitionByIdHandler : IRequestHandler<GetCompetitionByIdQuery, Result<GetCompetitionByIdResponse>>
{
    private readonly ICompetitionRepository _competitionRepository;

    public GetCompetitionByIdHandler(ICompetitionRepository competitionRepository)
    {
        _competitionRepository = competitionRepository;
    }

    public Task<Result<GetCompetitionByIdResponse>> Handle(GetCompetitionByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var competition = _competitionRepository.GetById(request.IdCompetition);

            if (competition == null)
            {
                return Task.FromResult(Result<GetCompetitionByIdResponse>.Failure($"Competition with ID {request.IdCompetition} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            var response = new GetCompetitionByIdResponse
            {
                IdCompetition = competition.IdCompetition,
                Name = competition.Name,
                StartedAt = competition.StartedAt,
                EndedAt = competition.EndedAt,
                NumberOfMatches = competition.NumberOfMatches,
                IsActive = competition.StartedAt <= today && (competition.EndedAt == null || competition.EndedAt >= today),
                Matches = competition.Matches.Select(m => new DetailedMatchInfo
                {
                    IdMatch = m.IdMatch,
                    Name = m.Name,
                    Date = m.CreatedAt,
                    Location = $"{m.City}, {m.Hall}",
                    Status = m.Type
                }).ToList()
            };

            return Task.FromResult(Result<GetCompetitionByIdResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetCompetitionByIdResponse>.Failure($"An error occurred while retrieving competition: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
