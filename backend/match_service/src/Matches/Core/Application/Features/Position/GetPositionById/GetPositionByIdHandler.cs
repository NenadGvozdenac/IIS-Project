using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;

namespace match_service.src.Matches.Core.Application.Features.Position.GetPositionById
{
    public class GetPositionByIdHandler : IRequestHandler<GetPositionByIdQuery, Result<GetPositionByIdResponse>>
    {
        private readonly IPositionRepository _positionRepository;

        public GetPositionByIdHandler(IPositionRepository positionRepository)
        {
            _positionRepository = positionRepository;
        }

        public Task<Result<GetPositionByIdResponse>> Handle(GetPositionByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var position = _positionRepository.GetById(request.Id);

                if (position == null)
                {
                    return Task.FromResult(Result<GetPositionByIdResponse>.Failure($"Position with ID {request.Id} not found.")
                        .WithCode((int)ResultCode.NotFound));
                }

                var response = new GetPositionByIdResponse
                {
                    Position = new PositionResponse
                    {
                        IdPosition = position.IdPosition,
                        Name = position.Name
                    }
                };

                return Task.FromResult(Result<GetPositionByIdResponse>.Success(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<GetPositionByIdResponse>.Failure($"An error occurred while retrieving position: {ex.Message}")
                    .WithCode((int)ResultCode.InternalServerError));
            }
        }
    }
}
