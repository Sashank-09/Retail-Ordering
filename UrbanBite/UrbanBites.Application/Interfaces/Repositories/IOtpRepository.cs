using UrbanBites.Domain.Entities;

namespace UrbanBites.Application.Interfaces.Repositories
{
    public interface IOtpRepository
    {
        Task<OtpRecord?> GetValidOtpAsync(string email, string otpCode);
        Task<IEnumerable<OtpRecord>> GetUnusedByEmailAsync(string email);
        Task AddAsync(OtpRecord record);
        void Update(OtpRecord record);
        Task<bool> SaveChangesAsync();
    }
}