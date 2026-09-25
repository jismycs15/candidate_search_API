using System.ComponentModel.DataAnnotations;

namespace Candiate_search_assesment.Model.Payload
{
    public class RefreshRequestDto
    {
        [Required(ErrorMessage = "RefreshToken is required.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
