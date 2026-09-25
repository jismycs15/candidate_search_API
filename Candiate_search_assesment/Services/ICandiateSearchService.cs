using Candiate_search_assesment.Model.Request;
using Candiate_search_assesment.Model.Response;

namespace Candiate_search_assesment.Services
{
    public interface ICandiateSearchService
    {
        Task<CandidateSearchResponseDto> SearchAsync(
            int currentCandidateId,
            CandidateSearchFilter filter,
            CancellationToken cancellationToken = default);
    }
}
