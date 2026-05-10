using UrbanBites.Application.DTOs.Auth;

namespace UrbanBites.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
        Task ForgotPasswordAsync(string email);
        Task<bool> VerifyOtpAsync(string email, string otpCode);
        Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
        Task<AuthResponseDto> GoogleSignInAsync(string googleToken);



    }
}