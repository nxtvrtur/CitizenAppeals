using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CitizenAppeals.Server.Model;
using CitizenAppeals.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CitizenAppeals.Server.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly CitizenAppealsContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(CitizenAppealsContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto? loginDto)
    {
        if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
        {
            return BadRequest("Username or password is empty");
        }

        var user = await _context.Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == loginDto.Username);
        if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            return Unauthorized("Неверный логин или пароль");
        }

        var token = GenerateJwtToken(user);
        return Ok(new TokenDto { Token = token });
    }

    private string GenerateJwtToken(User user)
    {
        var key = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(key))
        {
            throw new InvalidOperationException("JWT Key is not configured");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role?.Name ?? "User")
        };
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool VerifyPassword(string? loginDtoPassword, string userPasswordHash)
    {
        if (string.IsNullOrEmpty(loginDtoPassword) || string.IsNullOrEmpty(userPasswordHash))
            return false;

        using var sha256 = SHA256.Create();
        var computedHash = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(loginDtoPassword)))
            .Replace("-", "").ToUpper();
        return computedHash == userPasswordHash;
    }
}