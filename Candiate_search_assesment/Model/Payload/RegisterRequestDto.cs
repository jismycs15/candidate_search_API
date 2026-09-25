using System.ComponentModel.DataAnnotations;

namespace Candiate_search_assesment.Model.Payload
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit and one special character.")]
        public string Password { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Name must be at most 200 characters.")]
        public string? Name { get; set; }

        [Range(0, 120, ErrorMessage = "Age must be between 0 and 120.")]
        public int? Age { get; set; }

        [StringLength(50, ErrorMessage = "Gender must be at most 50 characters.")]
        public string? Gender { get; set; }

        [StringLength(200, ErrorMessage = "Location must be at most 200 characters.")]
        public string? Location { get; set; }

        [StringLength(200, ErrorMessage = "Education must be at most 200 characters.")]
        public string? Education { get; set; }
    }
}
