using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.TeamMembers.GetAllTeamMembers;

public class GetAllTeamMembersHandler : IRequestHandler<GetAllTeamMembersQuery, Result<List<GetAllTeamMembersResponse>>>
{
    private readonly ITeamMemberRepository _teamMemberRepository;

    public GetAllTeamMembersHandler(ITeamMemberRepository teamMemberRepository)
    {
        _teamMemberRepository = teamMemberRepository;
    }

    public Task<Result<List<GetAllTeamMembersResponse>>> Handle(GetAllTeamMembersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var teamMembers = _teamMemberRepository.GetAll();
            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            var response = teamMembers.Select(tm => new GetAllTeamMembersResponse
            {
                JerseyNumber = tm.JerseyNumber,
                Status = tm.Status,
                IdPlayer = tm.IdPlayer,
                IdTeam = tm.IdTeam
            }).ToList();

            return Task.FromResult(Result<List<GetAllTeamMembersResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetAllTeamMembersResponse>>.Failure($"An error occurred while retrieving team members: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}
