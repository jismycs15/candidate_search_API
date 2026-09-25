using System.Security.Claims;
using Candiate_search_assesment.Model.Request;
using Candiate_search_assesment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Candiate_search_assesment.Controllers
{
    [Route("api/candidates")]
    [ApiController]
    [Authorize]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandiateSearchService _candiateSearchService;

        public CandidatesController(ICandiateSearchService candiateSearchService)
        {
            _candiateSearchService = candiateSearchService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] int? minAge,
            [FromQuery] int? maxAge,
            [FromQuery] string? gender,
            [FromQuery] string? location,
            [FromQuery] string? education,
            [FromQuery] DateTime? createdFrom,
            [FromQuery] DateTime? createdTo,
            [FromQuery] DateTime? lastLoginFrom,
            [FromQuery] DateTime? lastLoginTo,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            CancellationToken cancellationToken)
        {
            var currentCandidateIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentCandidateIdClaim, out var currentCandidateId))
            {
                return Unauthorized(new { message = "Unable to identify the current candidate from the token." });
            }

            var filter = CandidateSearchFilter.FromQuery(
                minAge, maxAge, gender, location, education,
                createdFrom, createdTo, lastLoginFrom, lastLoginTo,
                sortBy, sortOrder, page, pageSize);

            var result = await _candiateSearchService.SearchAsync(currentCandidateId, filter, cancellationToken);

            return Ok(result);
        }
    }
}
