using UrbanBites.Application.DTOs.Order;
using UrbanBites.Application.DTOs.Coupon;
using UrbanBites.Application.DTOs.Loyalty;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Application.Interfaces.Services;
using UrbanBites.Application.Mappings;
using UrbanBites.Domain.Entities;
using UrbanBites.Domain.Enums;

namespace UrbanBites.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IEmailService _emailService;
        private readonly ICouponService _couponService;
        private readonly ILoyaltyService _loyaltyService;
        private readonly ICouponRepository _couponRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IEmailService emailService,
            ICouponService couponService,
            ILoyaltyService loyaltyService,
            ICouponRepository couponRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _emailService = emailService;
            _couponService = couponService;
            _loyaltyService = loyaltyService;
            _couponRepository = couponRepository;
        }

        public async Task<OrderDto> PlaceOrderAsync(
            Guid userId,
            string userEmail,
            string userName,
            PlaceOrderDto dto)
        {
            // 1. Validate cart
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart is null || !cart.CartItems.Any())
                throw new Exception("Cart is empty.");

            // 2. Build order items + deduct stock
            var orderItems = new List<OrderItem>();
            decimal subtotal = 0;

            foreach (var cartItem in cart.CartItems)
            {
                var product = await _productRepository
                    .GetByIdAsync(cartItem.ProductId);

                if (product is null)
                    throw new Exception(
                        $"Product not found: {cartItem.ProductId}");

                if (product.StockCount < cartItem.Quantity)
                    throw new Exception(
                        $"Insufficient stock for {product.Name}");

                product.StockCount -= cartItem.Quantity;
                _productRepository.Update(product);

                orderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    Quantity = cartItem.Quantity,
                    UnitPrice = product.Price
                });

                subtotal += product.Price * cartItem.Quantity;
            }

            // 3. Apply coupon discount
            decimal discountAmount = 0;
            string? appliedCoupon = null;

            if (!string.IsNullOrWhiteSpace(dto.CouponCode))
            {
                var couponResult = await _couponService.ApplyAsync(
                    new ApplyCouponDto
                    {
                        Code = dto.CouponCode,
                        OrderAmount = subtotal
                    });

                discountAmount = couponResult.DiscountAmount;
                appliedCoupon = dto.CouponCode.ToUpper();

                // Increment coupon usage count
                var coupon = await _couponRepository
                    .GetByCodeAsync(dto.CouponCode);
                if (coupon is not null)
                {
                    coupon.UsedCount++;
                    _couponRepository.Update(coupon);
                    await _couponRepository.SaveChangesAsync();
                }
            }

            // 4. Apply loyalty points redemption
            int loyaltyPointsUsed = 0;

            if (dto.UseLoyaltyPoints)
            {
                var balance = await _loyaltyService.GetBalanceAsync(userId);
                if (balance.TotalPoints > 0)
                {
                    var loyaltyDiscount = await _loyaltyService
                        .RedeemPointsAsync(userId, balance.TotalPoints);
                    discountAmount += loyaltyDiscount;
                    loyaltyPointsUsed = balance.TotalPoints;
                }
            }

            // 5. Calculate final total
            var total = Math.Max(0, subtotal - discountAmount);

            // 6. Create order
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Status = OrderStatus.Pending,
                TotalAmount = total,
                DiscountAmount = discountAmount,
                DeliveryAddress = dto.DeliveryAddress,
                SpecialRequests = dto.SpecialRequests,
                CouponCode = appliedCoupon,
                LoyaltyPointsUsed = loyaltyPointsUsed,
                LoyaltyPointsEarned = (int)total,
                TransactionId = dto.TransactionId,
                PlacedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            await _orderRepository.AddAsync(order);

            // 7. Capture product names BEFORE clearing cart
            var productNameMap = cart.CartItems
                .Where(ci => ci.Product != null)
                .ToDictionary(ci => ci.ProductId, ci => ci.Product!.Name);

            // 8. Clear cart
            cart.CartItems.Clear();
            await _orderRepository.SaveChangesAsync();

            // 9. Earn loyalty points
            await _loyaltyService.EarnPointsAsync(
                userId,
                total,
                $"Earned on order #{order.Id.ToString()[..8].ToUpper()}");

            // 10. Send confirmation email
            try
            {
                var emailItems = orderItems.Select(oi => new OrderEmailItem
                {
                    ProductName = productNameMap.TryGetValue(
                                      oi.ProductId, out var name) ? name : "Item",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    SubTotal = oi.UnitPrice * oi.Quantity
                }).ToList();

                Console.WriteLine($"[EMAIL] Attempting to send to: {userEmail}");
                Console.WriteLine($"[EMAIL] Customer name: {userName}");
                Console.WriteLine($"[EMAIL] Order ID: {order.Id}");
                Console.WriteLine($"[EMAIL] Items count: {emailItems.Count}");

                await _emailService.SendOrderConfirmationAsync(
                    userEmail,
                    userName,
                    order.Id.ToString(),
                    order.TotalAmount,
                    order.DiscountAmount,
                    order.DeliveryAddress,
                    emailItems,
                    order.TransactionId);

                Console.WriteLine($"[EMAIL] ✅ Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL] ❌ FAILED: {ex.Message}");
                Console.WriteLine($"[EMAIL] StackTrace: {ex.StackTrace}");
            }

            return order.ToDto();
        }

        public async Task<IEnumerable<OrderDto>> GetMyOrdersAsync(Guid userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            return orders.Select(o => o.ToDto());
        }

        public async Task<OrderDto?> GetOrderByIdAsync(Guid userId, Guid orderId)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(orderId);
            if (order is null || order.UserId != userId) return null;
            return order.ToDto();
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return orders.Select(o => o.ToDto());
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, string status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order is null) return false;

            if (!Enum.TryParse<OrderStatus>(status, true, out var parsed))
                throw new ArgumentException($"Invalid status: {status}");

            order.Status = parsed;
            _orderRepository.Update(order);
            return await _orderRepository.SaveChangesAsync();
        }
    }
}