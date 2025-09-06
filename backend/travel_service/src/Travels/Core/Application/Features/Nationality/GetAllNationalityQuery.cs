using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Nationality.GetAllNationality;

public class GetAllNationalityQuery : IRequest<Result<List<GetAllNationalityResponse>>>
{
}
