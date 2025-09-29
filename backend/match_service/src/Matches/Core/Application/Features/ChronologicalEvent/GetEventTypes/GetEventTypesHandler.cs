using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.ChronologicalEvent.GetEventTypes
{
    public class GetEventTypesHandler : IRequestHandler<GetEventTypesCommand, Result<GetEventTypesResponse>>
    {
        public Task<Result<GetEventTypesResponse>> Handle(GetEventTypesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var eventTypes = new List<string>();

                switch (request.Category.ToLower())
                {
                    case "personal":
                        eventTypes = new List<string>
                        {
                            "+2p", "+3p", "+ft", "2p", "3p", "assist", "block", 
                            "foul", "ft", "reb def", "reb of", "steal", 
                            "substitution in", "substitution out", "other"
                        };
                        break;
                    case "team":
                        eventTypes = new List<string>
                        {
                            "technical foul", "timeout", "formation", "defense", "other"
                        };
                        break;
                    case "general":
                        eventTypes = new List<string>
                        {
                            "break end", "break start", "pause end", 
                            "pause start", "period end", "period start", "other"
                        };
                        break;
                    default:
                        return Task.FromResult(Result<GetEventTypesResponse>.Failure("Invalid category. Supported categories: personal, team, general"));
                }

                var response = new GetEventTypesResponse
                {
                    EventTypes = eventTypes
                };

                return Task.FromResult(Result<GetEventTypesResponse>.Success(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<GetEventTypesResponse>.Failure($"Error retrieving event types: {ex.Message}"));
            }
        }
    }
}