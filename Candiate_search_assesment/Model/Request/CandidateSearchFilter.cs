namespace Candiate_search_assesment.Model.Request
{
    /// <summary>
    /// Normalized, validated representation of the /api/candidates/search query string.
    /// Parsing/clamping happens once here so the controller stays thin and the repository
    /// only ever sees clean, already-defaulted values.
    /// </summary>
    public class CandidateSearchFilter
    {
        private const int DefaultPageSize = 20;
        private const int MaxPageSize = 100;

        private static readonly HashSet<string> AllowedSortColumns =
            new(StringComparer.OrdinalIgnoreCase) { "createdAt", "lastLoginAt", "age" };

        public int? MinAge { get; private init; }
        public int? MaxAge { get; private init; }
        public string? Gender { get; private init; }
        public string? Location { get; private init; }
        public IReadOnlyList<string> EducationValues { get; private init; } = Array.Empty<string>();
        public DateTime? CreatedFrom { get; private init; }
        public DateTime? CreatedTo { get; private init; }
        public DateTime? LastLoginFrom { get; private init; }
        public DateTime? LastLoginTo { get; private init; }
        public string SortBy { get; private init; } = "createdAt";
        public string SortOrder { get; private init; } = "desc";
        public int Page { get; private init; } = 1;
        public int PageSize { get; private init; } = DefaultPageSize;

        public static CandidateSearchFilter FromQuery(
            int? minAge,
            int? maxAge,
            string? gender,
            string? location,
            string? education,
            DateTime? createdFrom,
            DateTime? createdTo,
            DateTime? lastLoginFrom,
            DateTime? lastLoginTo,
            string? sortBy,
            string? sortOrder,
            int? page,
            int? pageSize)
        {
            return new CandidateSearchFilter
            {
                MinAge = minAge,
                MaxAge = maxAge,
                Gender = string.IsNullOrWhiteSpace(gender) ? null : gender.Trim(),
                Location = string.IsNullOrWhiteSpace(location) ? null : location.Trim(),
                EducationValues = ParseEducation(education),
                CreatedFrom = ToUtcDate(createdFrom),
                CreatedTo = ToUtcDate(createdTo),
                LastLoginFrom = ToUtcDate(lastLoginFrom),
                LastLoginTo = ToUtcDate(lastLoginTo),
                SortBy = NormalizeSortBy(sortBy),
                SortOrder = NormalizeSortOrder(sortOrder),
                Page = page is > 0 ? page.Value : 1,
                PageSize = pageSize switch
                {
                    null or <= 0 => DefaultPageSize,
                    > MaxPageSize => MaxPageSize,
                    _ => pageSize.Value
                }
            };
        }

        // Splits "BTech,MTech,MBA" into distinct, trimmed values so the repository can
        // evaluate a single `WHERE education IN (...)` instead of per-value queries.
        private static IReadOnlyList<string> ParseEducation(string? education)
        {
            if (string.IsNullOrWhiteSpace(education))
                return Array.Empty<string>();

            return education
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static string NormalizeSortBy(string? sortBy) =>
            !string.IsNullOrWhiteSpace(sortBy) && AllowedSortColumns.Contains(sortBy)
                ? sortBy
                : "createdAt";

        private static string NormalizeSortOrder(string? sortOrder) =>
            string.Equals(sortOrder, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

        // created_at/last_login_at are stored as UTC timestamptz; query-string dates bind with
        // Kind=Unspecified, which Npgsql refuses to write against timestamptz. Stamping the Kind
        // here (rather than relying on the caller) keeps every date filter 200-safe.
        private static DateTime? ToUtcDate(DateTime? value) =>
            value is null ? null : DateTime.SpecifyKind(value.Value.Date, DateTimeKind.Utc);
    }
}
