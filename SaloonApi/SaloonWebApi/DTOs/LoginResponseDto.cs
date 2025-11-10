namespace SaloonWebApi.DTOs
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Token { get; set; } = string.Empty;          // 🔹 NEW
        public string RefreshToken { get; set; } = string.Empty;    // 🔹 NEW
    }


}