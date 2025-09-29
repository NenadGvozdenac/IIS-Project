namespace ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

public interface ICreditCardEncryptionService
{
    string EncryptCardNumber(string cardNumber);
    string DecryptCardNumber(string encryptedCardNumber);
    string EncryptCvv(string cvv);
    string DecryptCvv(string encryptedCvv);
    string MaskCardNumber(string cardNumber);
}
