using MediatR;
using travel_service.src.Travels.BuildingBlocks.Core.Domain;

namespace travel_service.src.Travels.Core.Application.Features.Trip.GetTravelCostReport;

public record GetTravelCostReportQuery() : IRequest<Result<TravelCostReportDto>>;