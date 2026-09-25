using Candiate_search_assesment.Model.Entity;
using Candiate_search_assesment.Model.Request;

namespace Candiate_search_assesment.Repository
{
    public interface ICandiateSearchRepo
    {
        /// <summary>
        /// Runs the filtered, sorted, paged candidate query entirely in PostgreSQL (a single
        /// count + a single page query) and excludes excludeCandidateId in the WHERE clause.
        /// </summary>
        Task<(List<Candidates> Items, int TotalCount)> SearchAsync(
            int excludeCandidateId,
            CandidateSearchFilter filter,
            CancellationToken cancellationToken = default);
    }
}
