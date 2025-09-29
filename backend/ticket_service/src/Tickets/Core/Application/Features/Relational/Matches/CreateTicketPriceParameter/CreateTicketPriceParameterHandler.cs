using MediatR;
using ticket_service.src.Tickets.BuildingBlocks.Core.Domain;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;
using ticket_service.src.Tickets.Core.Domain.Entities.Relational;

namespace ticket_service.src.Tickets.Core.Application.Features.Relational.Matches.CreateTicketPriceParameter;

public class CreateTicketPriceParameterHandler : IRequestHandler<CreateTicketPriceParameterQuery, Result<CreateTicketPriceParameterResponse>>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IZoneRepository _zoneRepository;
    private readonly ITicketPriceParameterRepository _ticketPriceParameterRepository;
    private readonly ISeatRepository _seatRepository;
    private readonly IUserRepository _userRepository;

    public CreateTicketPriceParameterHandler(
        IMatchRepository matchRepository,
        IZoneRepository zoneRepository,
        ITicketPriceParameterRepository ticketPriceParameterRepository,
        ISeatRepository seatRepository,
        IUserRepository userRepository)
    {
        _matchRepository = matchRepository;
        _zoneRepository = zoneRepository;
        _ticketPriceParameterRepository = ticketPriceParameterRepository;
        _seatRepository = seatRepository;
        _userRepository = userRepository;
    }

    public Task<Result<CreateTicketPriceParameterResponse>> Handle(CreateTicketPriceParameterQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate user is club owner
            var user = _userRepository.GetById(request.UserId);
            if (user == null)
            {
                return Task.FromResult(Result<CreateTicketPriceParameterResponse>.Failure($"User with ID {request.UserId} not found.")
                    .WithCode((int)ResultCode.NotFound));
            }

            if (user.Type != "club owner")
            {
                return Task.FromResult(Result<CreateTicketPriceParameterResponse>.Failure("Only club owners can create ticket price parameters.")
                    .WithCode((int)ResultCode.Forbidden));
            }

            // Validate match exists
            var match = _matchRepository.GetById(request.MatchId);
            if (match == null)
            {
                return Task.FromResult(Result<CreateTicketPriceParameterResponse>.Failure($"Match with ID {request.MatchId} not found.")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Validate zone exists
            var zone = _zoneRepository.GetById(request.ZoneId);
            if (zone == null)
            {
                return Task.FromResult(Result<CreateTicketPriceParameterResponse>.Failure($"Zone with ID {request.ZoneId} not found.")
                    .WithCode((int)ResultCode.NotFound));
            }

            // Validate that the zone has enabled seats
            var enabledSeatsInZone = _seatRepository.GetByZoneId(request.ZoneId)
                .Where(s => s.Status == "enabled")
                .ToList();

            if (!enabledSeatsInZone.Any())
            {
                return Task.FromResult(Result<CreateTicketPriceParameterResponse>.Failure($"Zone '{zone.Name}' has no enabled seats.")
                    .WithCode((int)ResultCode.BadRequest));
            }

            // Check if ticket price parameter already exists for this match and zone
            var existingParameter = _ticketPriceParameterRepository.GetByMatchZoneAndUser(request.MatchId, request.ZoneId);
            if (existingParameter != null)
            {
                return Task.FromResult(Result<CreateTicketPriceParameterResponse>.Failure($"Ticket price parameters already exist for zone '{zone.Name}' and this match.")
                    .WithCode((int)ResultCode.Conflict));
            }

            // Create new ticket price parameter
            var ticketPriceParameter = new TicketPriceParameter
            {
                PriceFactor = request.PriceFactor,
                TimeFactor = request.TimeFactor,
                MinimumSeatPrice = request.MinimumSeatPrice,
                MaximumSeatPrice = request.MaximumSeatPrice,
                IdUser = request.UserId,
                IdZone = request.ZoneId,
                IdMatch = request.MatchId
            };

            var createdParameter = _ticketPriceParameterRepository.Create(ticketPriceParameter);

            var response = new CreateTicketPriceParameterResponse
            {
                IdTicketPriceParameter = createdParameter.IdTicketPriceParameter,
                MatchId = match.IdMatch,
                MatchName = match.Name,
                ZoneId = zone.IdZone,
                ZoneName = zone.Name,
                PriceFactor = createdParameter.PriceFactor,
                TimeFactor = createdParameter.TimeFactor,
                MinimumSeatPrice = createdParameter.MinimumSeatPrice,
                MaximumSeatPrice = createdParameter.MaximumSeatPrice,
                UserId = createdParameter.IdUser,
                Message = $"Ticket price parameters have been successfully created for zone '{zone.Name}' and match '{match.Name}'."
            };

            return Task.FromResult(Result<CreateTicketPriceParameterResponse>.Success(response));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CreateTicketPriceParameterResponse>.Failure($"An error occurred while creating ticket price parameters: {ex.Message}")
                .WithCode((int)ResultCode.InternalServerError));
        }
    }
}