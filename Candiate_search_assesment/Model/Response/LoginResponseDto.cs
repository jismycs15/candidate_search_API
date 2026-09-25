namespace Candiate_search_assesment.Model.Response
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public int ExpiresIn { get; set; }

        public string RefreshToken { get; set; } = string.Empty;
    }
}
