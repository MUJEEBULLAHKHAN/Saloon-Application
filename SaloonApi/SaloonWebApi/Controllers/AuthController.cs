using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using SaloonWebApi.Data;
using SaloonWebApi.DTOs;

namespace SaloonWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SaloonDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(SaloonDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.UserNameOrEmail) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("username/email and password required.");

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.User_Name == dto.UserNameOrEmail || u.Email == dto.UserNameOrEmail);

            if (user == null || user.Active == 0)
                return Unauthorized("Invalid credentials.");

            // Plain-text check (works if your DB stores plain passwords)
            if (user.Password != dto.Password)
            {
                // If you store hashed passwords, verify here instead.
                // Example (requires BCrypt.Net-Next NuGet):
                // if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                //     return Unauthorized("Invalid credentials.");

                return Unauthorized("Invalid credentials.");
            }

        

            var response = new LoginResponseDto
            {
                UserId = user.User_Id,
                UserName = user.User_Name ?? string.Empty
            };

            return Ok(response);
        }
    }
}