namespace match_service.src.Matches.Core.Application.Features.Position.GetPositionById;

public class GetPositionByIdResponse
{
    public PositionResponse Position { get; set; }
}

public class PositionResponse
{
    public int IdPosition { get; set; }
    public string Name { get; set; }
}
