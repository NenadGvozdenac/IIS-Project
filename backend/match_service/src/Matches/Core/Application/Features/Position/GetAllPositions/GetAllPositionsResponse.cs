namespace match_service.src.Matches.Core.Application.Features.Position.GetAllPositions;

public class GetAllPositionsResponse
{
    public IEnumerable<PositionResponse> Positions { get; set; } = new List<PositionResponse>();
}

public class PositionResponse
{
    public int IdPosition { get; set; }
    public string? Name { get; set; }
}
