# Email Service Configuration

This service automatically sends email confirmations to customers after successful ticket purchases.

## Setup

### Gmail SMTP Configuration

1. **Enable 2-Factor Authentication** on your Gmail account
2. **Generate an App Password**:
   - Go to Google Account settings
   - Security → 2-Step Verification → App passwords
   - Create a new app password for "Mail"
3. **Configure appsettings.json**:

```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "User": "your-email@gmail.com",
    "Password": "your-16-character-app-password"
  }
}
```

### Template Structure

Email templates are stored in the `Templates/Email/` folder:

- `TicketPurchaseConfirmation.html` - Main email template
- `IndividualTicketItem.html` - Template for individual ticket items
- `SeasonTicketItem.html` - Template for season ticket items

### Template Variables

The following variables are automatically replaced in templates:

#### Main Template:
- `{{UserName}}` - Customer's first name
- `{{UserSurname}}` - Customer's last name
- `{{CartId}}` - Order/Cart ID
- `{{PurchaseDate}}` - Purchase date and time
- `{{TicketItems}}` - Generated HTML for all ticket items
- `{{TotalItems}}` - Number of items purchased
- `{{TotalAmount}}` - Total amount (formatted as currency)
- `{{Status}}` - Order status

#### Individual Ticket Template:
- `{{TicketName}}` - Ticket/offer name
- `{{MatchName}}` - Match name
- `{{MatchDateTime}}` - Match date and time
- `{{MatchVenue}}` - Venue (Hall, City)
- `{{ZoneName}}` - Zone name
- `{{ZoneId}}` - Zone ID
- `{{SeatRow}}` - Seat row number
- `{{SeatNumber}}` - Seat number
- `{{SeatId}}` - Seat ID
- `{{SeatDirection}}` - Seat direction
- `{{Price}}` - Item price (formatted as currency)

#### Season Ticket Template:
- `{{TicketName}}` - Ticket/offer name
- `{{SeasonName}}` - Season name
- `{{SeasonStart}}` - Season start date
- `{{SeasonEnd}}` - Season end date
- `{{ZoneName}}` - Zone name
- `{{ZoneId}}` - Zone ID
- `{{SeatRow}}` - Seat row number
- `{{SeatNumber}}` - Seat number
- `{{SeatId}}` - Seat ID
- `{{SeatDirection}}` - Seat direction
- `{{Price}}` - Item price (formatted as currency)

## Services

### IEmailService
Basic email sending functionality using SMTP.

### IEmailTemplateService
Template processing and email content generation.

### GmailEmailService
Implementation of IEmailService for Gmail SMTP.

### EmailTemplateService
Implementation of IEmailTemplateService that loads and processes HTML templates.

## How It Works

1. When a cart is purchased successfully
2. `PurchaseCartHandler` retrieves the user information
3. `EmailTemplateService` generates the email content using templates
4. `GmailEmailService` sends the email via Gmail SMTP
5. Customer receives a formatted HTML email with all ticket details

## Error Handling

Email sending errors are caught and ignored to prevent them from affecting the purchase process. In production, you should implement proper logging for email failures.

## Security Notes

- Never commit real email credentials to version control
- Use environment variables or Azure Key Vault for production
- Enable 2FA and use app passwords for Gmail
- Consider using Azure SendGrid or similar services for production environments