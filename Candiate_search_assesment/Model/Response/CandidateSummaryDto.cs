namespace Candiate_search_assesment.Model.Response
{
    public class CandidateSummaryDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        public string Education { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }
    }
}
