using MediatR;
using match_service.src.Matches.BuildingBlocks.Core.Domain;
using match_service.src.Matches.Core.Application.Interfaces;
using match_service.src.Matches.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using match_service.src.Matches.Core.Domain.Entities;


namespace match_service.src.Matches.Core.Application.Features.Player.CreatePlayerWithTeamMember;

public class CreatePlayerWithTeamMemberHandler : IRequestHandler<CreatePlayerWithTeamMemberCommand, Result<CreatePlayerWithTeamMemberResponse>>
{
    private readonly MatchDbContext _context;
    private readonly IPlayerRepository _playerRepository;
    private readonly ITeamMemberRepository _teamMemberRepository;

    public CreatePlayerWithTeamMemberHandler(
        MatchDbContext context,
        IPlayerRepository playerRepository, 
        ITeamMemberRepository teamMemberRepository)
    {
        _context = context;
        _playerRepository = playerRepository;
        _teamMemberRepository = teamMemberRepository;
    }

    public async Task<Result<CreatePlayerWithTeamMemberResponse>> Handle(CreatePlayerWithTeamMemberCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // Create Player
            var player = new Domain.Entities.Player
            {
                Name = request.Name,
                Surname = request.Surname,
                Birthday = request.Birthday,
                Weight = request.Weight,
                Height = request.Height,
                IdNationality = 1, // Fixed as required
                IdPosition = request.IdPosition
            };

            // Add player to context but don't save yet
            _context.Players.Add(player);
            await _context.SaveChangesAsync(cancellationToken);

            // Create TeamMember
            var teamMember = new Domain.Entities.TeamMember
            {
                IdPlayer = player.IdPlayer,
                IdTeam = request.IdTeam,
                JerseyNumber = request.JerseyNumber,
                Status = "active" // Fixed as required
            };

            // Add team member to context
            _context.TeamMembers.Add(teamMember);
            await _context.SaveChangesAsync(cancellationToken);

            // Commit transaction
            await transaction.CommitAsync(cancellationToken);

            var response = new CreatePlayerWithTeamMemberResponse
            {
                PlayerId = player.IdPlayer,
                PlayerName = player.Name,
                PlayerSurname = player.Surname,
                JerseyNumber = teamMember.JerseyNumber,
                Status = teamMember.Status
            };

            return Result<CreatePlayerWithTeamMemberResponse>.Success(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<CreatePlayerWithTeamMemberResponse>.Failure($"Failed to create player with team member: {ex.Message}");
        }
    }
}
