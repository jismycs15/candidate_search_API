using Candiate_search_assesment.Model.Request;
using Candiate_search_assesment.Model.Response;
using Candiate_search_assesment.Repository;

namespace Candiate_search_assesment.Services.impl
{
    public class CandiateSearchService : ICandiateSearchService
    {
        private readonly ICandiateSearchRepo _candiateSearchRepo;

        public CandiateSearchService(ICandiateSearchRepo candiateSearchRepo)
        {
            _candiateSearchRepo = candiateSearchRepo;
        }

        public async Task<CandidateSearchResponseDto> SearchAsync(
            int currentCandidateId,
            CandidateSearchFilter filter,
            CancellationToken cancellationToken = default)
        {
            var (items, totalCount) = await _candiateSearchRepo.SearchAsync(currentCandidateId, filter, cancellationToken);

            return new CandidateSearchResponseDto
            {
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                Items = items.Select(c => new CandidateSummaryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Age = c.Age,
                    Education = c.Education,
                    Location = c.Location,
                    CreatedAt = c.CreatedAt,
                    LastLoginAt = c.LastLoginAt
                }).ToList()
            };
        }
    }
}
