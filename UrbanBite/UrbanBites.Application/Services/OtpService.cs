using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Application.Interfaces.Services;
using UrbanBites.Domain.Entities;

namespace UrbanBites.Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        private readonly IOtpRepository _otpRepository;
        private readonly IEmailService _emailService;

        public OtpService(IOtpRepository otpRepository,
                          IEmailService emailService)
        {
            _otpRepository = otpRepository;
            _emailService = emailService;
        }

        public async Task<bool> SendOtpAsync(string email)
        {
            // Invalidate old OTPs
            var oldOtps = await _otpRepository.GetUnusedByEmailAsync(email);
            foreach (var old in oldOtps)
            {
                old.IsUsed = true;
                _otpRepository.Update(old);
            }

            // Generate 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();

            await _otpRepository.AddAsync(new OtpRecord
            {
                Id = Guid.NewGuid(),
                Email = email,
                OtpCode = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false
            });

            await _otpRepository.SaveChangesAsync();
            await _emailService.SendOtpEmailAsync(email, otp);
            return true;
        }

        public async Task<bool> VerifyOtpAsync(string email, string otpCode, bool markAsUsed = false)
        {
            var record = await _otpRepository
                .GetValidOtpAsync(email, otpCode);

            if (record is null) return false;

            if (markAsUsed)
            {
                record.IsUsed = true;
                _otpRepository.Update(record);
                await _otpRepository.SaveChangesAsync();
            }
            return true;
        }
    }
}