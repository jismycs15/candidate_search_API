using Candiate_search_assesment.DbContexts;
using Candiate_search_assesment.Model.Entity;
using Candiate_search_assesment.Model.Request;
using Microsoft.EntityFrameworkCore;

namespace Candiate_search_assesment.Repository.impl
{
    public class CandiateSearchRepo : ICandiateSearchRepo
    {
        private readonly CandidateDbContext _db;

        public CandiateSearchRepo(CandidateDbContext db)
        {
            _db = db;
        }

        public async Task<(List<Candidates> Items, int TotalCount)> SearchAsync(
            int excludeCandidateId,
            CandidateSearchFilter filter,
            CancellationToken cancellationToken = default)
        {
            var query = BuildFilteredQuery(excludeCandidateId, filter);
            var totalCount = await query.CountAsync(cancellationToken);

            var items = await ApplySort(query, filter.SortBy, filter.SortOrder)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        private IQueryable<Candidates> BuildFilteredQuery(int excludeCandidateId, CandidateSearchFilter filter)
        {
            var query = _db.Candidates.AsNoTracking()
                .Where(c => c.Id != excludeCandidateId);

            if (filter.MinAge.HasValue)
                query = query.Where(c => c.Age >= filter.MinAge.Value);

            if (filter.MaxAge.HasValue)
                query = query.Where(c => c.Age <= filter.MaxAge.Value);

            if (filter.Gender is not null)
                query = query.Where(c => EF.Functions.ILike(c.Gender, filter.Gender));

            if (filter.Location is not null)
                query = query.Where(c => EF.Functions.ILike(c.Location, $"%{filter.Location}%"));

            if (filter.EducationValues.Count > 0)
                query = query.Where(c => filter.EducationValues.Contains(c.Education));

            if (filter.CreatedFrom.HasValue)
                query = query.Where(c => c.CreatedAt >= filter.CreatedFrom.Value);

            if (filter.CreatedTo.HasValue)
                query = query.Where(c => c.CreatedAt < filter.CreatedTo.Value.AddDays(1));

            if (filter.LastLoginFrom.HasValue)
                query = query.Where(c => c.LastLoginAt.HasValue && c.LastLoginAt >= filter.LastLoginFrom.Value);

            if (filter.LastLoginTo.HasValue)
                query = query.Where(c => c.LastLoginAt.HasValue && c.LastLoginAt < filter.LastLoginTo.Value.AddDays(1));

            return query;
        }

        private static IQueryable<Candidates> ApplySort(IQueryable<Candidates> query, string sortBy, string sortOrder)
        {
            var descending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

            return sortBy.ToLowerInvariant() switch
            {
                "lastloginat" => descending ? query.OrderByDescending(c => c.LastLoginAt) : query.OrderBy(c => c.LastLoginAt),
                "age" => descending ? query.OrderByDescending(c => c.Age) : query.OrderBy(c => c.Age),
                _ => descending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt)
            };
        }
    }
}
