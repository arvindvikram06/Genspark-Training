namespace BankingAPI.Models.DTOs
{
    public class TokenRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}