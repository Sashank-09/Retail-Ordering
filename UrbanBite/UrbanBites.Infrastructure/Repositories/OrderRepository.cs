using Microsoft.EntityFrameworkCore;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Domain.Entities;
using UrbanBites.Infrastructure.Data;

namespace UrbanBites.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId)
            => await _dbSet
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.PlacedAt)
                .ToListAsync();

        public async Task<Order?> GetOrderWithItemsAsync(Guid orderId)
            => await _dbSet
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
            => await _dbSet
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.PlacedAt)
            .ToListAsync();

    }
}