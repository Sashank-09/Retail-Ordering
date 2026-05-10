using Microsoft.EntityFrameworkCore;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Domain.Entities;
using UrbanBites.Infrastructure.Data;

namespace UrbanBites.Infrastructure.Repositories
{
    public class OtpRepository : IOtpRepository
    {
        private readonly AppDbContext _context;

        public OtpRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OtpRecord?> GetValidOtpAsync(string email, string otpCode)
            => await _context.OtpRecords
                .Where(o => o.Email == email &&
                            o.OtpCode == otpCode &&
                            !o.IsUsed &&
                            o.ExpiresAt > DateTime.UtcNow)
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<OtpRecord>> GetUnusedByEmailAsync(string email)
            => await _context.OtpRecords
                .Where(o => o.Email == email && !o.IsUsed)
                .ToListAsync();

        public async Task AddAsync(OtpRecord record)
            => await _context.OtpRecords.AddAsync(record);

        public void Update(OtpRecord record)
            => _context.OtpRecords.Update(record);

        public async Task<bool> SaveChangesAsync()
            => await _context.SaveChangesAsync() > 0;
    }
}