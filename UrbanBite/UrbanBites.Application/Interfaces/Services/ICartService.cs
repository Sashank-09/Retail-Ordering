using UrbanBites.Application.DTOs.Cart;

namespace UrbanBites.Application.Interfaces.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(Guid userId);
        Task<CartDto> AddToCartAsync(Guid userId, AddToCartDto dto);
        Task<CartDto> UpdateCartItemAsync(Guid userId, Guid cartItemId, UpdateCartItemDto dto);
        Task<bool> RemoveCartItemAsync(Guid userId, Guid cartItemId);
        Task<bool> ClearCartAsync(Guid userId);
    }
}