using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Graph;

namespace ticket_service.src.Tickets.Core.Application.Features.Graph.Matches.DeleteMatch;

public class DeleteGraphMatchHandler : IRequestHandler<DeleteGraphMatchCommand, Result<DeleteGraphMatchResponse>>
{
    private readonly IGraphMatchRepository _graphMatchRepository;

    public DeleteGraphMatchHandler(IGraphMatchRepository graphMatchRepository)
    {
        _graphMatchRepository = graphMatchRepository;
    }

    public async Task<Result<DeleteGraphMatchResponse>> Handle(DeleteGraphMatchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _graphMatchRepository.DeleteMatch(request.Id);

            var response = new DeleteGraphMatchResponse
            {
                Message = $"Match with ID '{request.Id}' deleted successfully"
            };

            return Result<DeleteGraphMatchResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<DeleteGraphMatchResponse>.Failure($"An error occurred while deleting the match: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}