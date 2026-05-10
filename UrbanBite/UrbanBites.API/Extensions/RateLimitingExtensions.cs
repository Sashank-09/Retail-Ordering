using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace UrbanBites.API.Extensions
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddRateLimitingPolicies(
            this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                // ── Global fallback ───────────────────────
                options.RejectionStatusCode = 429;

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsync(
                        """{"statusCode":429,"message":"Too many requests. Please slow down."}""",
                        token);
                };

                // ── General API Policy ─────────────────────
                // 30 requests per minute per IP
                options.AddFixedWindowLimiter("GeneralPolicy", opt =>
                {
                    opt.PermitLimit = 30;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 5;
                });

                // ── Auth Policy ────────────────────────────
                // 5 login/register attempts per minute per IP
                // Prevents brute force attacks
                options.AddFixedWindowLimiter("AuthPolicy", opt =>
                {
                    opt.PermitLimit = 5;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                // ── Order Policy ───────────────────────────
                // 10 orders per minute per IP
                options.AddFixedWindowLimiter("OrderPolicy", opt =>
                {
                    opt.PermitLimit = 10;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 2;
                });
            });

            return services;
        }
    }
}