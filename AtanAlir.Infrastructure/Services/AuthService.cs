using AtanAlir.Core.DTOs;
using AtanAlir.Core.Entities;
using AtanAlir.Core.Interfaces;
using AtanAlir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;


namespace AtanAlir.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly TokenService _tokenService;
    private readonly EmailService _emailService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ApplicationDbContext context,
        IConfiguration configuration,
        TokenService tokenService,
        EmailService emailService,
        ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _tokenService = tokenService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<UserDto> RegisterAsync(RegisterDto model)
    {
        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            throw new Exception("Email already exists");

        var user = new User
        {
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PasswordHash = HashPassword(model.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Doğrulama kodu oluştur ve gönder
        await SendVerificationCodeAsync(user.Email);

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Nickname = user.Nickname,
            IsEmailVerified = user.IsEmailVerified
        };
    }

    public async Task<string> LoginAsync(LoginDto model)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null || user.PasswordHash != HashPassword(model.Password))
            throw new Exception("Invalid email or password");

        return _tokenService.GenerateToken(user);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private async Task SendVerificationCodeAsync(string email)
    {
        var code = Random.Shared.Next(100000, 999999).ToString();

        var verificationCode = new VerificationCode
        {
            Email = email,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            CreatedAt = DateTime.UtcNow
        };

        _context.VerificationCodes.Add(verificationCode);
        await _context.SaveChangesAsync();

        await _emailService.SendVerificationEmailAsync(email, code);
    }

    public async Task<bool> VerifyEmailAsync(string email, string code)
    {
        var verification = await _context.VerificationCodes
            .Where(v => v.Email == email && v.Code == code && !v.IsUsed)
            .OrderByDescending(v => v.CreatedAt)
            .FirstOrDefaultAsync();

        if (verification == null || verification.ExpiresAt < DateTime.UtcNow)
            return false;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return false;

        verification.IsUsed = true;
        user.IsEmailVerified = true;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SetNicknameAsync(Guid userId, string nickname)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new Exception("User not found");

        if (await _context.Users.AnyAsync(u => u.Nickname == nickname))
            throw new Exception("Nickname is already taken");

        if (user.Nickname != null)
            throw new Exception("Nickname can only be set once");

        user.Nickname = nickname;
        await _context.SaveChangesAsync();
        return true;
    }
}