using System.Security.Cryptography;
using System.Text;
using ticket_service.src.Tickets.Core.Application.Interfaces.Relational;

namespace ticket_service.src.Tickets.Core.Infrastructure.Services;

public class CreditCardEncryptionService : ICreditCardEncryptionService
{
    private readonly string _encryptionKey;
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public CreditCardEncryptionService(IConfiguration configuration)
    {
        _encryptionKey = configuration["Encryption:CreditCardKey"] ?? "MySecretKey12345MySecretKey12345"; // 32 characters for AES-256
        
        // Generate key and IV from the encryption key
        using (var sha256 = SHA256.Create())
        {
            _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(_encryptionKey));
        }
        
        // Use first 16 bytes of key as IV
        _iv = new byte[16];
        Array.Copy(_key, 0, _iv, 0, 16);
    }

    public string EncryptCardNumber(string cardNumber)
    {
        if (string.IsNullOrEmpty(cardNumber))
            return string.Empty;

        return EncryptString(cardNumber);
    }

    public string DecryptCardNumber(string encryptedCardNumber)
    {
        if (string.IsNullOrEmpty(encryptedCardNumber))
            return string.Empty;

        return DecryptString(encryptedCardNumber);
    }

    public string EncryptCvv(string cvv)
    {
        if (string.IsNullOrEmpty(cvv))
            return string.Empty;

        return EncryptString(cvv);
    }

    public string DecryptCvv(string encryptedCvv)
    {
        if (string.IsNullOrEmpty(encryptedCvv))
            return string.Empty;

        return DecryptString(encryptedCvv);
    }

    public string MaskCardNumber(string cardNumber)
    {
        if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 4)
            return cardNumber;

        return "**** **** **** " + cardNumber.Substring(cardNumber.Length - 4);
    }

    private string EncryptString(string plainText)
    {
        try
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                using (var msEncrypt = new MemoryStream())
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                    swEncrypt.Close();
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error encrypting data", ex);
        }
    }

    private string DecryptString(string cipherText)
    {
        try
        {
            var cipherBytes = Convert.FromBase64String(cipherText);

            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor())
                using (var msDecrypt = new MemoryStream(cipherBytes))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error decrypting data", ex);
        }
    }
}
