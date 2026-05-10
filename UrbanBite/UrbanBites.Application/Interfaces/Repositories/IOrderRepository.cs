using UrbanBites.Domain.Entities;

namespace UrbanBites.Application.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
        Task<Order?> GetOrderWithItemsAsync(Guid orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
    }
}