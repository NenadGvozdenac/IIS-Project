using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.Competitions.GetCompetitionById;

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
            var competition = _competitionRepository.GetById(request.Id);

            if (competition == null)
            {
                return Task.FromResult(Result<GetCompetitionByIdResponse>.Failure($"Competition with ID {request.Id} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            var response = new GetCompetitionByIdResponse
            {
                IdCompetition = competition.IdCompetition,
                Name = competition.Name,
                StartedAt = competition.StartedAt,
                EndedAt = competition.EndedAt
            };

            return Task.FromResult(Result<GetCompetitionByIdResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<GetCompetitionByIdResponse>.Failure($"An error occurred while retrieving the competition: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
