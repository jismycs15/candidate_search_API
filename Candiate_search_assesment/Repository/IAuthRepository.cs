using Candiate_search_assesment.Model.Entity;

namespace Candiate_search_assesment.Repository
{
    public interface IAuthRepository
    {
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

        Task<Candidates?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        Task<Candidates?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<Candidates?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken cancellationToken = default);

        /// <summary>Sets (or, passing nulls, clears) the candidate's current refresh token.</summary>
        Task SetRefreshTokenAsync(int candidateId, string? refreshTokenHash, DateTime? expiresAtUtc, CancellationToken cancellationToken = default);

        /// <summary>Persists a new candidate. Throws EmailAlreadyExistsException on a unique-constraint violation.</summary>
        Task AddCandidateAsync(Candidates candidate, CancellationToken cancellationToken = default);

        Task UpdateLastLoginAsync(int candidateId, DateTime loginTimeUtc, CancellationToken cancellationToken = default);
    }
}
