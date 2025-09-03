using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.ManagementMembers.GetAllMembers;

public class GetAllMembersQuery : IRequest<Result<List<GetAllMembersResponse>>>
{
}