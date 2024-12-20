using AtanAlir.Core.DTOs;

namespace AtanAlir.Core.Interfaces;

// AtanAlir.Core/Interfaces/IAuthService.cs

public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterDto model);
    Task<string> LoginAsync(LoginDto model);
    Task<bool> VerifyEmailAsync(string email, string code);
    Task<bool> SetNicknameAsync(Guid userId, string nickname);
}