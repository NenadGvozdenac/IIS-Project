using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;
using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Competitions.CreateCompetition;

public class CreateCompetitionHandler : IRequestHandler<CreateCompetitionCommand, Result<CreateCompetitionResponse>>
{
    private readonly ICompetitionRepository _competitionRepository;

    public CreateCompetitionHandler(ICompetitionRepository competitionRepository)
    {
        _competitionRepository = competitionRepository;
    }

    public Task<Result<CreateCompetitionResponse>> Handle(CreateCompetitionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validacija
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Task.FromResult(Result<CreateCompetitionResponse>.Failure("Competition name is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (request.EndedAt.HasValue && request.EndedAt < request.StartedAt)
            {
                return Task.FromResult(Result<CreateCompetitionResponse>.Failure("End date cannot be before start date")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Proveri da li već postoji takmičenje sa istim nazivom
            if (_competitionRepository.ExistsByName(request.Name))
            {
                return Task.FromResult(Result<CreateCompetitionResponse>.Failure($"Competition with name '{request.Name}' already exists")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Kreiraj novo takmičenje
            var competition = new Competition
            {
                Name = request.Name,
                StartedAt = request.StartedAt,
                EndedAt = request.EndedAt,
                NumberOfMatches = request.NumberOfMatches
            };

            var createdCompetition = _competitionRepository.Create(competition);

            var response = new CreateCompetitionResponse
            {
                IdCompetition = createdCompetition.IdCompetition,
                Name = createdCompetition.Name,
                StartedAt = createdCompetition.StartedAt,
                EndedAt = createdCompetition.EndedAt,
                NumberOfMatches = createdCompetition.NumberOfMatches,
                Message = "Competition created successfully"
            };

            return Task.FromResult(Result<CreateCompetitionResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreateCompetitionResponse>.Failure($"An error occurred while creating competition: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
