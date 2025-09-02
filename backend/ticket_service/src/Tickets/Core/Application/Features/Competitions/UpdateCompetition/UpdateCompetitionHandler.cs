using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Competitions.UpdateCompetition;

public class UpdateCompetitionHandler : IRequestHandler<UpdateCompetitionCommand, Result<UpdateCompetitionResponse>>
{
    private readonly ICompetitionRepository _competitionRepository;

    public UpdateCompetitionHandler(ICompetitionRepository competitionRepository)
    {
        _competitionRepository = competitionRepository;
    }

    public Task<Result<UpdateCompetitionResponse>> Handle(UpdateCompetitionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validacija
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Task.FromResult(Result<UpdateCompetitionResponse>.Failure("Competition name is required")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (request.NumberOfMatches <= 0)
            {
                return Task.FromResult(Result<UpdateCompetitionResponse>.Failure("Number of matches must be greater than 0")
                    .WithCode((int)ResultCode.BadRequest));
            }

            if (request.EndedAt.HasValue && request.EndedAt < request.StartedAt)
            {
                return Task.FromResult(Result<UpdateCompetitionResponse>.Failure("End date cannot be before start date")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Pronađi postojeće takmičenje
            var existingCompetition = _competitionRepository.GetById(request.IdCompetition);
            if (existingCompetition == null)
            {
                return Task.FromResult(Result<UpdateCompetitionResponse>.Failure($"Competition with ID {request.IdCompetition} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Proveri da li već postoji drugo takmičenje sa istim nazivom
            if (_competitionRepository.ExistsByNameExcludingId(request.Name, request.IdCompetition))
            {
                return Task.FromResult(Result<UpdateCompetitionResponse>.Failure($"Another competition with name '{request.Name}' already exists")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Ažuriraj takmičenje
            existingCompetition.Name = request.Name;
            existingCompetition.StartedAt = request.StartedAt;
            existingCompetition.EndedAt = request.EndedAt;
            existingCompetition.NumberOfMatches = request.NumberOfMatches;

            var updatedCompetition = _competitionRepository.Update(existingCompetition);

            var response = new UpdateCompetitionResponse
            {
                IdCompetition = updatedCompetition.IdCompetition,
                Name = updatedCompetition.Name,
                StartedAt = updatedCompetition.StartedAt,
                EndedAt = updatedCompetition.EndedAt,
                NumberOfMatches = updatedCompetition.NumberOfMatches,
                Message = "Competition updated successfully"
            };

            return Task.FromResult(Result<UpdateCompetitionResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<UpdateCompetitionResponse>.Failure($"An error occurred while updating competition: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
