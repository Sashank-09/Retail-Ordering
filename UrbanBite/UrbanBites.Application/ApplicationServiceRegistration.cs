using Microsoft.Extensions.DependencyInjection;
using UrbanBites.Application.Interfaces.Services;
using UrbanBites.Application.Services;

namespace UrbanBites.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // ── Services ──────────────────────────────────
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<ILoyaltyService, LoyaltyService>();

            return services;
        }
    }
}