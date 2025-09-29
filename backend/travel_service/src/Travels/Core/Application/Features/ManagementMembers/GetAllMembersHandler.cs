using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;
using travel_service.src.Travels.Core.Application.Interfaces;

namespace travel_service.src.Travels.Core.Application.Features.ManagementMembers.GetAllMembers;

public class GetAllMembersHandler : IRequestHandler<GetAllMembersQuery, Result<List<GetAllMembersResponse>>>
{
    private readonly IMemberRepository _memberRepository;

    public GetAllMembersHandler(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public Task<Result<List<GetAllMembersResponse>>> Handle(GetAllMembersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var members = _memberRepository.GetAll();
            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            var response = members.Select(m => new GetAllMembersResponse
            {
                MemberId = m.MemberId,
                MemberName = m.MemberName,
                MemberSurname = m.MemberSurname,
                MemberRole = m.MemberRole
            }).ToList();

            return Task.FromResult(Result<List<GetAllMembersResponse>>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<List<GetAllMembersResponse>>.Failure($"An error occurred while retrieving members: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}

