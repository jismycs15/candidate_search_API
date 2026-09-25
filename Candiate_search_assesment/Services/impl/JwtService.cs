using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Candiate_search_assesment.Model.Entity;
using Microsoft.IdentityModel.Tokens;

namespace Candiate_search_assesment.Services.impl
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string Token, int ExpiresIn) GenerateToken(Candidates candidate)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expiresInMinutes = int.TryParse(jwtSection["ExpiresInMinutes"], out var minutes) ? minutes : 60;

            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(expiresInMinutes);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, candidate.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, candidate.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, candidate.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Report the token's actual remaining lifetime so expiresIn always matches the exp claim.
            var expiresIn = (int)(expires - now).TotalSeconds;

            return (tokenString, expiresIn);
        }

        public string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        public TimeSpan RefreshTokenLifetime
        {
            get
            {
                var days = int.TryParse(_configuration["Jwt:RefreshTokenExpiresInDays"], out var d) ? d : 7;
                return TimeSpan.FromDays(days);
            }
        }
    }
}
