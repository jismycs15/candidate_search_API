using Candiate_search_assesment.Model.Payload;
using Candiate_search_assesment.Model.Response;

namespace Candiate_search_assesment.Services
{
    public interface IAuthService
    {
        Task<RegisterResultDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);

        /// <summary>Returns null on invalid credentials — caller returns 401 without saying which part was wrong.</summary>
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

        /// <summary>Exchanges a valid, unexpired refresh token for a new access token and rotates the refresh token. Returns null if invalid/expired.</summary>
        Task<LoginResponseDto?> RefreshAsync(RefreshRequestDto request, CancellationToken cancellationToken = default);

        Task LogoutAsync(int candidateId, CancellationToken cancellationToken = default);
    }
}
