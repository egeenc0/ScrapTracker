using Microsoft.AspNetCore.Mvc;
using HurdaApi.Data;
using HurdaApi.Dtos;
using HurdaApi.Services;
using Microsoft.EntityFrameworkCore;

namespace HurdaApi.Controllers;

[ApiController]
[Route("api/[controller]")]              // → /api/auth  (elle, küçük)

public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;
    public AuthController(AppDbContext context, TokenService tokenService){
        _context = context;
        _tokenService = tokenService;
    }
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto request)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);

        if (user == null || user.PasswordHash != request.Password)
            return Unauthorized("Kullanıcı adı veya şifre hatalı");

        var token = _tokenService.GenerateToken(user);

        return Ok(new LoginResponseDto
        {
            Token = token,
            Username = user.Username,
            Role = user.Role
        });
    }

}