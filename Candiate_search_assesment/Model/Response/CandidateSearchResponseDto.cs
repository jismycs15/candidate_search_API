namespace Candiate_search_assesment.Model.Response
{
    public class CandidateSearchResponseDto
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public List<CandidateSummaryDto> Items { get; set; } = new();
    }
}
