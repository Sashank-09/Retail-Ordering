using Microsoft.EntityFrameworkCore;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Domain.Entities;
using UrbanBites.Infrastructure.Data;

namespace UrbanBites.Infrastructure.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context) { }

        public async Task<Cart?> GetCartByUserIdAsync(Guid userId)
            => await _dbSet
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

        public async Task AddCartItemAsync(CartItem item)
        {
            await _context.CartItems.AddAsync(item);
        }
    }

}