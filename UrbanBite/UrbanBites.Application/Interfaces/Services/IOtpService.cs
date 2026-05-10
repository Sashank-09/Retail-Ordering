namespace UrbanBites.Application.Interfaces.Services
{
    public interface IOtpService
    {
        Task<bool> SendOtpAsync(string email);
        Task<bool> VerifyOtpAsync(string email, string otpCode, bool markAsUsed = false);
    }
}