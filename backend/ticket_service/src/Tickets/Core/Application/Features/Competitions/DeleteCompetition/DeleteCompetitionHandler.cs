using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces;

namespace ticket_service.src.Tickets.Core.Application.Features.Competitions.DeleteCompetition;

public class DeleteCompetitionHandler : IRequestHandler<DeleteCompetitionCommand, Result<DeleteCompetitionResponse>>
{
    private readonly ICompetitionRepository _competitionRepository;

    public DeleteCompetitionHandler(ICompetitionRepository competitionRepository)
    {
        _competitionRepository = competitionRepository;
    }

    public Task<Result<DeleteCompetitionResponse>> Handle(DeleteCompetitionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pronađi postojeće takmičenje
            var existingCompetition = _competitionRepository.GetById(request.IdCompetition);
            if (existingCompetition == null)
            {
                return Task.FromResult(Result<DeleteCompetitionResponse>.Failure($"Competition with ID {request.IdCompetition} not found")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Proveri da li takmičenje ima mečove
            if (existingCompetition.Matches.Any())
            {
                return Task.FromResult(Result<DeleteCompetitionResponse>.Failure("Cannot delete competition that has matches")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Obriši takmičenje
            _competitionRepository.Delete(request.IdCompetition);

            var response = new DeleteCompetitionResponse
            {
                Message = $"Competition '{existingCompetition.Name}' deleted successfully"
            };

            return Task.FromResult(Result<DeleteCompetitionResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<DeleteCompetitionResponse>.Failure($"An error occurred while deleting competition: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
