namespace Clinic_Management.Models
{
    public record class JwtOptions(
     string Issuer,
     string Audience,
     string SigningKey,
     int ExpirationSeconds
 );
}
