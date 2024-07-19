using System;
using System.Security.Cryptography;
using System.Text;

public static class TokenMail
{
    private static readonly string Secret = "G1SecretKey";

    public static string GenerateToken(int userId, string email)
    {
        var expiryTime = DateTime.UtcNow.AddMinutes(1);
        var data = $"{userId}:{email}:{expiryTime:yyyyMMddHHmmss}";
        var encodedData = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        var hash = ComputeHash(encodedData);
        return $"{encodedData}.{hash}";
    }

    public static (bool isValid, int userId, string email) ValidateToken(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 2) return (false, 0, null);

            var encodedData = parts[0];
            var hash = parts[1];

            if (ComputeHash(encodedData) != hash) return (false, 0, null);

            var decodedData = Encoding.UTF8.GetString(Convert.FromBase64String(encodedData));
            var dataParts = decodedData.Split(':');
            if (dataParts.Length != 3) return (false, 0, null);

            var userId = int.Parse(dataParts[0]);
            var email = dataParts[1];
            var expiryDate = DateTime.ParseExact(dataParts[2], "yyyyMMddHHmmss", null);

            if (expiryDate < DateTime.UtcNow) return (false, 0, null);

            return (true, userId, email);
        }
        catch
        {
            return (false, 0, null);
        }
    }

    private static string ComputeHash(string input)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Secret)))
        {
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }
    }
}