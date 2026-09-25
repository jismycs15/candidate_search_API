using System.Security.Cryptography;
using System.Text;
using Candiate_search_assesment.Exceptions;
using Candiate_search_assesment.Model.Entity;
using Candiate_search_assesment.Model.Payload;
using Candiate_search_assesment.Model.Response;
using Candiate_search_assesment.Repository;

namespace Candiate_search_assesment.Services.impl
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtService _jwtService;

        public AuthService(IAuthRepository authRepository, IJwtService jwtService)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
        }

        public async Task<RegisterResultDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            if (await _authRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
            {
                return RegisterResultDto.Conflict("An account with this email already exists.");
            }

            var candidate = new Candidates
            {
                Email = normalizedEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Name = request.Name?.Trim() ?? string.Empty,
                Age = request.Age ?? 0,
                Gender = request.Gender?.Trim() ?? string.Empty,
                Location = request.Location?.Trim() ?? string.Empty,
                Education = request.Education?.Trim() ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _authRepository.AddCandidateAsync(candidate, cancellationToken);
            }
            catch (EmailAlreadyExistsException)
            {
                return RegisterResultDto.Conflict("An account with this email already exists.");
            }

            return RegisterResultDto.Ok(new CandidateProfileDto
            {
                Id = candidate.Id,
                Email = candidate.Email,
                Name = candidate.Name
            });
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var candidate = await _authRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

            if (candidate is null || !VerifyPassword(request.Password, candidate.PasswordHash))
            {
                return null;
            }

            var (token, expiresIn) = _jwtService.GenerateToken(candidate);
            var refreshToken = await IssueRefreshTokenAsync(candidate.Id, cancellationToken);

            await _authRepository.UpdateLastLoginAsync(candidate.Id, DateTime.UtcNow, cancellationToken);

            return new LoginResponseDto { Token = token, ExpiresIn = expiresIn, RefreshToken = refreshToken };
        }

        public async Task<LoginResponseDto?> RefreshAsync(RefreshRequestDto request, CancellationToken cancellationToken = default)
        {
            var candidate = await _authRepository.GetByRefreshTokenHashAsync(HashToken(request.RefreshToken), cancellationToken);

            if (candidate is null || candidate.RefreshTokenExpiresAt is null || candidate.RefreshTokenExpiresAt <= DateTime.UtcNow)
            {
                return null;
            }

            var (token, expiresIn) = _jwtService.GenerateToken(candidate);
            // Rotate: the old refresh token's hash is overwritten, so it can't be replayed.
            var newRefreshToken = await IssueRefreshTokenAsync(candidate.Id, cancellationToken);

            return new LoginResponseDto { Token = token, ExpiresIn = expiresIn, RefreshToken = newRefreshToken };
        }

        public Task LogoutAsync(int candidateId, CancellationToken cancellationToken = default) =>
            _authRepository.SetRefreshTokenAsync(candidateId, null, null, cancellationToken);

        private async Task<string> IssueRefreshTokenAsync(int candidateId, CancellationToken cancellationToken)
        {
            var refreshToken = _jwtService.GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.Add(_jwtService.RefreshTokenLifetime);

            await _authRepository.SetRefreshTokenAsync(candidateId, HashToken(refreshToken), expiresAt, cancellationToken);

            return refreshToken;
        }

        // Refresh tokens are high-entropy random values, not user-chosen passwords, so a fast
        // cryptographic hash (rather than BCrypt) is enough to keep a leaked DB from being replayable.
        private static string HashToken(string token) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        // BCrypt.Net throws (rather than returning false) when a stored hash isn't a well-formed
        // bcrypt hash — e.g. a row seeded with placeholder data. A malformed hash should read as
        // "wrong password", never surface as a 500.
        private static bool VerifyPassword(string password, string passwordHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
