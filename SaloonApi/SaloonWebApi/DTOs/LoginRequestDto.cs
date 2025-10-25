namespace SaloonWebApi.DTOs
{
    public class LoginRequestDto
    {
        // Accept either username or email in this field
        public string UserNameOrEmail { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}