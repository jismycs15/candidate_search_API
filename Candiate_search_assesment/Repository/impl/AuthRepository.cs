using Candiate_search_assesment.DbContexts;
using Candiate_search_assesment.Exceptions;
using Candiate_search_assesment.Model.Entity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Candiate_search_assesment.Repository.impl
{
    public class AuthRepository : IAuthRepository
    {
        private readonly CandidateDbContext _db;

        public AuthRepository(CandidateDbContext db)
        {
            _db = db;
        }

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default) =>
            _db.Candidates.AsNoTracking().AnyAsync(c => c.Email == email, cancellationToken);

        public Task<Candidates?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
            _db.Candidates.FirstOrDefaultAsync(c => c.Email == email, cancellationToken);

        public Task<Candidates?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
            _db.Candidates.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public Task<Candidates?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken cancellationToken = default) =>
            _db.Candidates.FirstOrDefaultAsync(c => c.RefreshTokenHash == refreshTokenHash, cancellationToken);

        public Task SetRefreshTokenAsync(int candidateId, string? refreshTokenHash, DateTime? expiresAtUtc, CancellationToken cancellationToken = default) =>
            _db.Candidates
                .Where(c => c.Id == candidateId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.RefreshTokenHash, refreshTokenHash)
                    .SetProperty(c => c.RefreshTokenExpiresAt, expiresAtUtc), cancellationToken);

        public async Task AddCandidateAsync(Candidates candidate, CancellationToken cancellationToken = default)
        {
            _db.Candidates.Add(candidate);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (IsUniqueEmailViolation(ex))
            {
                throw new EmailAlreadyExistsException(candidate.Email, ex);
            }
        }

        public Task UpdateLastLoginAsync(int candidateId, DateTime loginTimeUtc, CancellationToken cancellationToken = default) =>
            _db.Candidates
                .Where(c => c.Id == candidateId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.LastLoginAt, loginTimeUtc), cancellationToken);

        private static bool IsUniqueEmailViolation(DbUpdateException ex) =>
            ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    }
}
