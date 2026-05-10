using UrbanBites.Application.DTOs.Brand;
using UrbanBites.Application.DTOs.Category;
using UrbanBites.Application.DTOs.Product;
using UrbanBites.Application.DTOs.Cart;
using UrbanBites.Application.DTOs.Order;
using UrbanBites.Domain.Entities;

namespace UrbanBites.Application.Mappings
{
    public static class MappingExtensions
    {
        // ── Brand ────────────────────────────────────────
        public static BrandDto ToDto(this Brand brand) => new()
        {
            Id = brand.Id,
            Name = brand.Name,
            LogoUrl = brand.LogoUrl
        };

        public static Brand ToEntity(this CreateBrandDto dto) => new()
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            LogoUrl = dto.LogoUrl
        };

        public static void UpdateEntity(this UpdateBrandDto dto, Brand brand)
        {
            brand.Name = dto.Name;
            brand.LogoUrl = dto.LogoUrl;
        }

        // ── Category ─────────────────────────────────────
        public static CategoryDto ToDto(this Category category) => new()
        {
            Id = category.Id,
            Name = category.Name,
            BrandName = category.Brand?.Name ?? string.Empty
        };

        public static Category ToEntity(this CreateCategoryDto dto) => new()
        {
            Id = Guid.NewGuid(),
            BrandId = dto.BrandId,
            Name = dto.Name
        };

        public static void UpdateEntity(this UpdateCategoryDto dto, Category category)
        {
            category.Name = dto.Name;
        }

        // ── Product ──────────────────────────────────────
        public static ProductDto ToDto(this Product product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockCount = product.StockCount,
            ImageUrl = product.ImageUrl,
            Packaging = product.Packaging,
            CategoryName = product.Category?.Name ?? string.Empty,
            BrandName = product.Brand?.Name ?? string.Empty
        };

        public static Product ToEntity(this CreateProductDto dto) => new()
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockCount = dto.StockCount,
            ImageUrl = dto.ImageUrl,
            Packaging = dto.Packaging,
            CategoryId = dto.CategoryId,
            BrandId = dto.BrandId
        };

        public static void UpdateEntity(this UpdateProductDto dto, Product product)
        {
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockCount = dto.StockCount;
            product.ImageUrl = dto.ImageUrl;
            product.Packaging = dto.Packaging;

        }

        // ── Cart ─────────────────────────────────────────
        public static CartDto ToDto(this Cart cart) => new()
        {
            Id = cart.Id,
            Items = cart.CartItems.Select(ci => ci.ToDto()).ToList()
        };

        public static CartItemDto ToDto(this CartItem cartItem) => new()
        {
            CartItemId = cartItem.Id,
            ProductId = cartItem.ProductId,
            ProductName = cartItem.Product?.Name ?? string.Empty,
            ImageUrl = cartItem.Product?.ImageUrl ?? string.Empty,
            UnitPrice = cartItem.Product?.Price ?? 0,
            Quantity = cartItem.Quantity
        };

        // ── Order ─────────────────────────────────────────
        // ✅ Fixed
        public static OrderDto ToDto(this Order order) => new()
        {
            Id = order.Id,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            DiscountAmount = order.DiscountAmount,        // ✅
            CouponCode = order.CouponCode,                // ✅
            LoyaltyPointsEarned = order.LoyaltyPointsEarned, // ✅
            LoyaltyPointsUsed = order.LoyaltyPointsUsed,  // ✅
            DeliveryAddress = order.DeliveryAddress,
            PlacedAt = order.PlacedAt,
            SpecialRequests = order.SpecialRequests,
            Items = order.OrderItems.Select(oi => oi.ToDto()).ToList()
        };

        public static OrderItemDto ToDto(this OrderItem orderItem) => new()
        {
            ProductId = orderItem.ProductId,
            ProductName = orderItem.Product?.Name ?? string.Empty,
            Quantity = orderItem.Quantity,
            UnitPrice = orderItem.UnitPrice,
            SubTotal = orderItem.UnitPrice * orderItem.Quantity  // ✅
        };
    }
}