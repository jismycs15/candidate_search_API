using Candiate_search_assesment.Model.Entity;

namespace Candiate_search_assesment.Services
{
    public interface IJwtService
    {
        /// <summary>Issues a signed JWT for the candidate. ExpiresIn is in seconds and matches the token's exp claim.</summary>
        (string Token, int ExpiresIn) GenerateToken(Candidates candidate);

        /// <summary>Generates a high-entropy opaque refresh token. Callers store only its hash.</summary>
        string GenerateRefreshToken();

        /// <summary>How long a refresh token stays valid, read from Jwt:RefreshTokenExpiresInDays.</summary>
        TimeSpan RefreshTokenLifetime { get; }
    }
}
