using UrbanBites.Application.DTOs.Order;

namespace UrbanBites.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderDto> PlaceOrderAsync(Guid userId, string userEmail,string userName, PlaceOrderDto dto);
        Task<IEnumerable<OrderDto>> GetMyOrdersAsync(Guid userId);
        Task<OrderDto?> GetOrderByIdAsync(Guid userId, Guid orderId);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, string status);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    }
}