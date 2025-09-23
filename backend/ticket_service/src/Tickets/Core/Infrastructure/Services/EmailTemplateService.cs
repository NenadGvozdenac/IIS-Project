using System.Text;
using ticket_service.src.Tickets.Core.Domain.Entities.Relational;
using Microsoft.Extensions.Hosting;

namespace ticket_service.src.Tickets.Core.Infrastructure.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IHostEnvironment _hostEnvironment;

        public EmailTemplateService(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public async Task<string> GenerateTicketPurchaseConfirmationAsync(User user, Cart cart, decimal totalAmount)
        {
            try
            {
                var templatePath = Path.Combine(_hostEnvironment.ContentRootPath, "Templates", "Email", "TicketPurchaseConfirmation.html");

                var template = await File.ReadAllTextAsync(templatePath);

                // Generate ticket items HTML
                var ticketItemsHtml = new StringBuilder();
                if (cart.CartItems != null)
                {
                    foreach (var cartItem in cart.CartItems)
                    {
                        try
                        {
                            var ticketHtml = await GenerateTicketItemHtmlAsync(cartItem);
                            ticketItemsHtml.Append(ticketHtml);
                        }
                        catch (Exception ex)
                        {
                            ticketItemsHtml.Append($"<p>Error loading ticket item: {ex.Message}</p>");
                        }
                    }
                }

                // Replace placeholders in main template
                var html = template
                    .Replace("{{UserName}}", user?.Name ?? "Customer")
                    .Replace("{{UserSurname}}", user?.Surname ?? "")
                    .Replace("{{CartId}}", cart?.IdCart.ToString() ?? "0")
                    .Replace("{{PurchaseDate}}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                    .Replace("{{TicketItems}}", ticketItemsHtml.ToString())
                    .Replace("{{TotalItems}}", cart?.ItemsNumber.ToString() ?? "0")
                    .Replace("{{TotalAmount}}", totalAmount.ToString())
                    .Replace("{{Status}}", cart?.Status?.ToUpper() ?? "UNKNOWN");

                return html;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to generate email template: {ex.Message}", ex);
            }
        }

        private async Task<string> GenerateTicketItemHtmlAsync(CartItem cartItem)
        {
            var purchaseOffer = cartItem.IdPurchaseOfferNavigation;
            if (purchaseOffer == null) return string.Empty;

            var seat = purchaseOffer.IdSeatNavigation;
            if (seat == null) return string.Empty;

            var zone = seat.IdZoneNavigation;

            if (purchaseOffer.IndividualTicket != null)
            {
                return await GenerateIndividualTicketHtmlAsync(cartItem, purchaseOffer, seat, zone);
            }
            else if (purchaseOffer.SeasonTicket != null)
            {
                return await GenerateSeasonTicketHtmlAsync(cartItem, purchaseOffer, seat, zone);
            }

            return string.Empty;
        }

        private async Task<string> GenerateIndividualTicketHtmlAsync(CartItem cartItem, PurchaseOffer purchaseOffer, Seat seat, Zone? zone)
        {
            var templatePath = Path.Combine(_hostEnvironment.ContentRootPath, "Templates", "Email", "IndividualTicketItem.html");
            var template = await File.ReadAllTextAsync(templatePath);

            var individualTicket = purchaseOffer.IndividualTicket!;
            var match = individualTicket.IdMatchNavigation;

            if (match == null)
            {
                return $"<p>Individual Ticket - {purchaseOffer.Name} - Unable to load match details</p>";
            }

            return template
                .Replace("{{TicketName}}", purchaseOffer.Name ?? "Unknown")
                .Replace("{{MatchName}}", match.Name ?? "Unknown Match")
                .Replace("{{MatchDateTime}}", match.ScheduledAt.ToString("dd/MM/yyyy HH:mm"))
                .Replace("{{MatchVenue}}", $"{match.Hall ?? "Unknown Hall"}, {match.City ?? "Unknown City"}")
                .Replace("{{ZoneName}}", zone?.Name ?? "N/A")
                .Replace("{{ZoneId}}", (zone?.IdZone ?? 0).ToString())
                .Replace("{{SeatRow}}", seat.Row.ToString())
                .Replace("{{SeatNumber}}", seat.Number.ToString())
                .Replace("{{SeatId}}", seat.IdSeat.ToString())
                .Replace("{{SeatDirection}}", seat.Direction ?? "Unknown")
                .Replace("{{Price}}", cartItem.Price.ToString());
        }

        private async Task<string> GenerateSeasonTicketHtmlAsync(CartItem cartItem, PurchaseOffer purchaseOffer, Seat seat, Zone? zone)
        {
            var templatePath = Path.Combine(_hostEnvironment.ContentRootPath, "Templates", "Email", "SeasonTicketItem.html");
            var template = await File.ReadAllTextAsync(templatePath);

            var seasonTicket = purchaseOffer.SeasonTicket!;
            var season = seasonTicket.IdSeasonNavigation;

            if (season == null)
            {
                return $"<p>Season Ticket - {purchaseOffer.Name} - Unable to load season details</p>";
            }

            return template
                .Replace("{{TicketName}}", purchaseOffer.Name ?? "Unknown")
                .Replace("{{SeasonName}}", season.Name ?? "Unknown Season")
                .Replace("{{SeasonStart}}", season.StartedAt.ToString("dd/MM/yyyy"))
                .Replace("{{SeasonEnd}}", season.EndedAt?.ToString("dd/MM/yyyy") ?? "Ongoing")
                .Replace("{{ZoneName}}", zone?.Name ?? "N/A")
                .Replace("{{ZoneId}}", (zone?.IdZone ?? 0).ToString())
                .Replace("{{SeatRow}}", seat.Row.ToString())
                .Replace("{{SeatNumber}}", seat.Number.ToString())
                .Replace("{{SeatId}}", seat.IdSeat.ToString())
                .Replace("{{SeatDirection}}", seat.Direction ?? "Unknown")
                .Replace("{{Price}}", cartItem.Price.ToString());
        }
    }
}