using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SaloonWebApi.Data;
using SaloonWebApi.DTOs;
using SaloonWebApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

            // Plain-text password check (use hashing later if you decide)
            if (user.Password != dto.Password)
                return Unauthorized("Invalid credentials.");

            // Create tokens
            var accessToken = GenerateJwtToken(user);
            var refreshToken = Guid.NewGuid().ToString();

            // Persist refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(GetRefreshDays());
            await _db.SaveChangesAsync();

            var response = new LoginResponseDto
            {
                UserId = user.User_Id,
                UserName = user.User_Name ?? string.Empty,
                Token = accessToken,
                RefreshToken = refreshToken
            };

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.RefreshToken))
                return BadRequest("Refresh token required.");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.RefreshToken == dto.RefreshToken);

            if (user == null || user.RefreshTokenExpiry == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token.");

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = Guid.NewGuid().ToString();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(GetRefreshDays());
            await _db.SaveChangesAsync();

            return Ok(new
            {
                token = newAccessToken,
                refreshToken = newRefreshToken
            });
        }

        [NonAction]
        private string GenerateJwtToken(User user)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.User_Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.User_Name ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
            };

            // NOTE: You can also add roles here if needed later

            var expires = DateTime.UtcNow.AddHours(GetAccessTokenHours());

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
       
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequestDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.RefreshToken == dto.RefreshToken);
            if (user == null) return Ok();

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _db.SaveChangesAsync();

            return Ok("Logged out successfully.");
        }

        [NonAction]
        private int GetAccessTokenHours()
            => int.TryParse(_config["Jwt:AccessTokenHours"], out var h) ? h : 2;

        [NonAction]
        private int GetRefreshDays()
            => int.TryParse(_config["Jwt:RefreshDays"], out var d) ? d : 7;
    }
}