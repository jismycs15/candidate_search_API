namespace Candiate_search_assesment.Model.Response
{
    public class RegisterResultDto
    {
        public bool Success { get; set; }

        public bool IsConflict { get; set; }

        public string? ErrorMessage { get; set; }

        public CandidateProfileDto? Candidate { get; set; }

        public static RegisterResultDto Conflict(string errorMessage) =>
            new() { Success = false, IsConflict = true, ErrorMessage = errorMessage };

        public static RegisterResultDto Ok(CandidateProfileDto candidate) =>
            new() { Success = true, Candidate = candidate };
    }
}
