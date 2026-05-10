using UrbanBites.Application.DTOs.Cart;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Application.Interfaces.Services;
using UrbanBites.Application.Mappings;
using UrbanBites.Domain.Entities;

namespace UrbanBites.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository,
                           IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<CartDto> GetCartAsync(Guid userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            return cart is null ? new CartDto() : cart.ToDto();
        }

        public async Task<CartDto> AddToCartAsync(Guid userId, AddToCartDto dto)
        {
            var product = await _productRepository.GetByIdAsync(dto.ProductId);
            if (product is null)
                throw new Exception("Product not found.");
            if (product.StockCount < dto.Quantity)
                throw new Exception("Insufficient stock.");

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart is null)
            {
                // ✅ Save cart FIRST by itself
                var cartId = Guid.NewGuid();
                cart = new Cart
                {
                    Id = cartId,
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                await _cartRepository.AddAsync(cart);
                await _cartRepository.SaveChangesAsync(); // ✅ Cart saved cleanly

                // ✅ Now add CartItem separately — EF knows cart exists
                var newItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cartId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };
                await _cartRepository.AddCartItemAsync(newItem); // ✅ Explicit INSERT
                await _cartRepository.SaveChangesAsync();

                // Reload to get navigation properties for ToDto()
                cart = await _cartRepository.GetCartByUserIdAsync(userId);
                return cart!.ToDto();
            }

            // Existing cart path
            var existingItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == dto.ProductId);

            if (existingItem is not null)
                existingItem.Quantity += dto.Quantity;
            else
            {
                var newItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };
                await _cartRepository.AddCartItemAsync(newItem); // ✅ Explicit INSERT
            }

            await _cartRepository.SaveChangesAsync();
            return (await _cartRepository.GetCartByUserIdAsync(userId))!.ToDto();
        }

        public async Task<CartDto> UpdateCartItemAsync(Guid userId, Guid cartItemId,
                                                       UpdateCartItemDto dto)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart is null) throw new Exception("Cart not found.");

            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item is null) throw new Exception("Cart item not found.");

            item.Quantity = dto.Quantity;
            await _cartRepository.SaveChangesAsync();
            return cart.ToDto();
        }

        public async Task<bool> RemoveCartItemAsync(Guid userId, Guid cartItemId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart is null) return false;

            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item is null) return false;

            cart.CartItems.Remove(item);
            return await _cartRepository.SaveChangesAsync();
        }

        public async Task<bool> ClearCartAsync(Guid userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart is null) return false;

            cart.CartItems.Clear();
            return await _cartRepository.SaveChangesAsync();
        }
    }
}