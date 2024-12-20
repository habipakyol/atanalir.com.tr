using AtanAlir.Core.DTOs;
using AtanAlir.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AtanAlir.Core.Models;
using AtanAlir.Infrastructure.Data;
using AtanAlir.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace AtanAlir.API.Controllers;


[Authorize] // Controller seviyesinde authorize
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly TokenService _tokenService;

    public AuthController(IAuthService authService, ApplicationDbContext context,
        IConfiguration configuration,
        TokenService tokenService)
    {
        _authService = authService;
        _context = context;
        _configuration = configuration;
        _tokenService = tokenService;
    }

    [AllowAnonymous] // Register için anonim erişime izin ver
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto model)
    {
        try
        {
            var result = await _authService.RegisterAsync(model);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [AllowAnonymous] // Login için anonim erişime izin ver
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginDto model)
    {
        try
        {
            var token = await _authService.LoginAsync(model);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [AllowAnonymous] // Email doğrulama için anonim erişime izin ver
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] EmailVerificationDto model)
    {
        var result = await _authService.VerifyEmailAsync(model.Email, model.Code);
        if (result)
            return Ok("Email successfully verified");

        return BadRequest("Invalid or expired verification code");
    }

    // Nickname belirleme için authentication gerekli
    [AllowAnonymous] 
    [HttpPost("set-nickname")]
    public async Task<IActionResult> SetNickname([FromBody] SetNicknameRequest request)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return NotFound("Kullanıcı bulunamadı");

            // Nickname kontrolü
            var nicknameExists = await _context.Users
                .AnyAsync(u => u.Nickname == request.Nickname);

            if (nicknameExists)
                return BadRequest("Bu nickname zaten kullanımda");

            user.Nickname = request.Nickname;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Nickname başarıyla belirlendi" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}