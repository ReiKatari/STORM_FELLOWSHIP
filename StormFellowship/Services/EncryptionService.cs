using System.Security.Cryptography;
using System.Text;

namespace StormFellowship.Services;

public class EncryptionService
{
    private static EncryptionService? _instance;
    public static EncryptionService Instance => _instance ??= new EncryptionService();

    private readonly byte[] _sessionKey;
    public string Fingerprint { get; }

    public EncryptionService()
    {
        _sessionKey = new byte[32]; // AES-256
        RandomNumberGenerator.Fill(_sessionKey);

        // Generate 6-chunk verification fingerprint (Safety numbers)
        using var sha = SHA256.Create();
        byte[] hash = sha.ComputeHash(_sessionKey);
        var sb = new StringBuilder();
        for (int i = 0; i < 6; i++)
        {
            int val = (hash[i * 4] << 8 | hash[i * 4 + 1]) % 10000;
            sb.Append($"{val:D4}");
            if (i < 5) sb.Append("-");
        }
        Fingerprint = sb.ToString();
    }

    public string EncryptString(string plainText)
    {
        try
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(bytes);
        }
        catch
        {
            return plainText;
        }
    }

    public string DecryptString(string cipherText)
    {
        try
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;
            byte[] bytes = Convert.FromBase64String(cipherText);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return cipherText;
        }
    }
}
