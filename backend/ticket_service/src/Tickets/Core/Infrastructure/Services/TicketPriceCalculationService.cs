using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;
using ticket_service.src.Tickets.Core.Infrastructure;

namespace ticket_service.src.Tickets.Core.Infrastructure.Services;

public class TicketPriceCalculationService : ITicketPriceCalculationService
{
    private readonly TicketDbContext _context;
    private const decimal DefaultMinPrice = 1000m; // Default fallback price

    public TicketPriceCalculationService(TicketDbContext context)
    {
        _context = context;
    }

    public decimal CalculateTicketPrice(int matchId, int zoneId)
    {
        try
        {
            var connection = _context.Database.GetDbConnection();
            
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT calculate_ticket_price($1, $2)";
            
            var matchParam = command.CreateParameter();
            matchParam.Value = matchId;
            command.Parameters.Add(matchParam);
            
            var zoneParam = command.CreateParameter();
            zoneParam.Value = zoneId;
            command.Parameters.Add(zoneParam);
            
            var result = command.ExecuteScalar();
            var price = Convert.ToDecimal(result);
            
            return price;
        }
        catch (Exception ex)
        {
            // Log error and return default price
            Console.WriteLine($"Error calculating ticket price: {ex.Message}");
            return DefaultMinPrice;
        }
    }
}
