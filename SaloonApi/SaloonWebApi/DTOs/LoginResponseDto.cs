namespace SaloonWebApi.DTOs
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
    }
}