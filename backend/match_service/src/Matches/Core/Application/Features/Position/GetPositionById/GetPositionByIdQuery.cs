using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;

namespace match_service.src.Matches.Core.Application.Features.Position.GetPositionById
{
    public class GetPositionByIdQuery : IRequest<Result<GetPositionByIdResponse>>
    {
        public int Id { get; set; }

        public GetPositionByIdQuery(int id)
        {
            Id = id;
        }
    }
}
