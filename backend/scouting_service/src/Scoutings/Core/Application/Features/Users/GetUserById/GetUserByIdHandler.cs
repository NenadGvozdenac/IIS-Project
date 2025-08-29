using MediatR;
using scouting_service.src.Scoutings.BuildingBlocks.Core.Domain;
using scouting_service.src.Scoutings.Core.Application.Interfaces;

namespace scouting_service.src.Scoutings.Core.Application.Features.Users.GetUserById;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdResponse>>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<GetUserByIdResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.Id);

            if (user == null)
            {
                return Result<GetUserByIdResponse>.Failure($"User with ID {request.Id} not found")
                    .WithCode((int)ResultCode.NotFound);
            }

            var response = new GetUserByIdResponse
            {
                Id_User = user.Id_User,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Phone = user.Phone,
                Type = user.Type
            };

            return Result<GetUserByIdResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetUserByIdResponse>.Failure($"An error occurred while retrieving the user: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError);
        }
    }
}
