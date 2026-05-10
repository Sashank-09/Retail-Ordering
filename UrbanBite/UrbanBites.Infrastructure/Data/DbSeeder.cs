using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrbanBites.Domain.Entities;
using UrbanBites.Infrastructure.Identity;

namespace UrbanBites.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(
            AppDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            await SeedRolesAsync(roleManager);
            await SeedOwnerAsync(userManager);
            await SeedAdminAsync(userManager);
            await SeedCatalogueAsync(context);
            await SeedPromotionsAsync(context);
            await SeedCouponsAsync(context);
        }

        // ── Roles ─────────────────────────────────────────
        private static async Task SeedRolesAsync(
            RoleManager<AppRole> roleManager)
        {
            foreach (var role in new[] { "Owner", "Admin", "Customer" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(
                        new AppRole { Name = role });
            }
        }

        // ── Owner User ────────────────────────────────────
        private static async Task SeedOwnerAsync(
            UserManager<AppUser> userManager)
        {
            const string ownerEmail = "owner@urbanbites.com";
            if (await userManager.FindByEmailAsync(ownerEmail) is not null)
                return;

            var owner = new AppUser
            {
                FullName = "Platform Owner",
                Email = ownerEmail,
                UserName = ownerEmail,
                PhoneNumber = "9999999990"
            };

            var result = await userManager.CreateAsync(owner, "Owner@1234");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(owner, "Owner");
        }

        // ── Admin User ────────────────────────────────────
        private static async Task SeedAdminAsync(
            UserManager<AppUser> userManager)
        {
            const string adminEmail = "admin@urbanbites.com";
            if (await userManager.FindByEmailAsync(adminEmail) is not null)
                return;

            var admin = new AppUser
            {
                FullName = "Super Admin",
                Email = adminEmail,
                UserName = adminEmail,
                PhoneNumber = "9999999999"
            };

            var result = await userManager.CreateAsync(admin, "Admin@1234");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // ── Catalogue ─────────────────────────────────────
        private static async Task SeedCatalogueAsync(AppDbContext context)
        {
            if (await context.Brands.AnyAsync()) return;

            // Brands
            var dominos = new Brand
            {
                Id = Guid.NewGuid(),
                Name = "Domino's",
                LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3e/Domino%27s_pizza_logo.svg/200px-Domino%27s_pizza_logo.svg.png",
                CreatedAt = DateTime.UtcNow
            };

            var kfc = new Brand
            {
                Id = Guid.NewGuid(),
                Name = "KFC",
                LogoUrl = "https://upload.wikimedia.org/wikipedia/en/thumb/b/bf/KFC_logo.svg/200px-KFC_logo.svg.png",
                CreatedAt = DateTime.UtcNow
            };

            var subway = new Brand
            {
                Id = Guid.NewGuid(),
                Name = "Subway",
                LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/5c/Subway_2016_logo.svg/200px-Subway_2016_logo.svg.png",
                CreatedAt = DateTime.UtcNow
            };

            await context.Brands.AddRangeAsync(dominos, kfc, subway);
            await context.SaveChangesAsync();

            // Categories
            var pizzas = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Pizzas",
                BrandId = dominos.Id,
                CreatedAt = DateTime.UtcNow
            };
            var sides = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Sides",
                BrandId = dominos.Id,
                CreatedAt = DateTime.UtcNow
            };
            var burgers = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Burgers",
                BrandId = kfc.Id,
                CreatedAt = DateTime.UtcNow
            };
            var chicken = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Chicken",
                BrandId = kfc.Id,
                CreatedAt = DateTime.UtcNow
            };
            var subs = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Subs",
                BrandId = subway.Id,
                CreatedAt = DateTime.UtcNow
            };

            await context.Categories.AddRangeAsync(
                pizzas, sides, burgers, chicken, subs);
            await context.SaveChangesAsync();

            // Products
            await context.Products.AddRangeAsync(
                // Domino's Pizzas
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Margherita Pizza",
                    Description = "Classic pizza with fresh tomato sauce, " +
                                  "mozzarella cheese and basil.",
                    Price = 249.00m,
                    StockCount = 50,
                    ImageUrl = "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?w=400",
                    Packaging = "Box",
                    CategoryId = pizzas.Id,
                    BrandId = dominos.Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Pepperoni Pizza",
                    Description = "Loaded with spicy pepperoni slices " +
                                  "on a classic tomato base.",
                    Price = 349.00m,
                    StockCount = 40,
                    ImageUrl = "https://images.unsplash.com/photo-1628840042765-356cda07504e?w=400",
                    Packaging = "Box",
                    CategoryId = pizzas.Id,
                    BrandId = dominos.Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "BBQ Chicken Pizza",
                    Description = "Smoky BBQ sauce with grilled chicken " +
                                  "and caramelised onions.",
                    Price = 379.00m,
                    StockCount = 35,
                    ImageUrl = "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400",
                    Packaging = "Box",
                    CategoryId = pizzas.Id,
                    BrandId = dominos.Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Garlic Bread",
                    Description = "Toasted bread with garlic butter " +
                                  "and herbs. Perfect side dish.",
                    Price = 129.00m,
                    StockCount = 80,
                    ImageUrl = "https://images.unsplash.com/photo-1619531040576-f9416740661f?w=400",
                    Packaging = "Bag",
                    CategoryId = sides.Id,
                    BrandId = dominos.Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Choco Lava Cake",
                    Description = "Warm chocolate cake with a gooey " +
                                  "molten center.",
                    Price = 99.00m,
                    StockCount = 60,
                    ImageUrl = "https://images.unsplash.com/photo-1606313564200-e75d5e30476c?w=400",
                    Packaging = "Box",
                    CategoryId = sides.Id,
                    BrandId = dominos.Id,
                    CreatedAt = DateTime.UtcNow
                },

                // KFC
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Zinger Burger",
                    Description = "Crispy spicy chicken fillet with " +
                                  "lettuce and mayo in a toasted bun.",
                    Price = 199.00m,
                    StockCount = 60,
                    ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=400",
                    Packaging = "Wrapper",
                    CategoryId = burgers.Id,
                    BrandId = kfc.Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Double Crunch Burger",
                    Description = "Two crispy chicken patties with " +
                                  "extra crunch and special sauce.",
                    Price = 269.00m,
                    StockCount = 45,
                    ImageUrl = "https://images.unsplash.com/photo-1553979459-d2229ba7433b?w=400",
                    Packaging = "Wrapper",
                    CategoryId = burgers.Id,
                    BrandId = kfc.Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Chicken Popcorn",
                    Description = "Bite-sized crispy chicken pieces " +
                                  "seasoned with KFC secret spices.",
                    Price = 169.00m,
                    StockCount = 100,
                    ImageUrl = "https://images.unsplash.com/photo-1562967914-608f82629710?w=400",
                    Packaging = "Box",
                    CategoryId = chicken.Id,
                    BrandId = kfc.Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Hot Wings (6 pcs)",
                    Description = "Spicy hot chicken wings marinated " +
                                  "in KFC signature sauce.",
                    Price = 219.00m,
                    StockCount = 75,
                    ImageUrl = "https://images.unsplash.com/photo-1567620832903-9fc6debc209f?w=400",
                    Packaging = "Box",
                    CategoryId = chicken.Id,
                    BrandId = kfc.Id,
                    CreatedAt = DateTime.UtcNow
                },

                // Subway
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Veggie Delite Sub",
                    Description = "Fresh vegetables on freshly baked " +
                                  "bread with your choice of sauce.",
                    Price = 189.00m,
                    StockCount = 70,
                    ImageUrl = "https://images.unsplash.com/photo-1509722747041-616f39b57569?w=400",
                    Packaging = "Wrap",
                    CategoryId = subs.Id,
                    BrandId = subway.Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Chicken Teriyaki Sub",
                    Description = "Tender chicken strips glazed with " +
                                  "teriyaki sauce on Italian bread.",
                    Price = 259.00m,
                    StockCount = 55,
                    ImageUrl = "https://images.unsplash.com/photo-1528735602780-2552fd46c7af?w=400",
                    Packaging = "Wrap",
                    CategoryId = subs.Id,
                    BrandId = subway.Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Steak & Cheese Sub",
                    Description = "Tender steak strips with melted " +
                                  "cheese and sautéed onions.",
                    Price = 299.00m,
                    StockCount = 40,
                    ImageUrl = "https://images.unsplash.com/photo-1554433607-66b5efe9d304?w=400",
                    Packaging = "Wrap",
                    CategoryId = subs.Id,
                    BrandId = subway.Id,
                    CreatedAt = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();
        }

        // ── Promotions ────────────────────────────────────
        private static async Task SeedPromotionsAsync(AppDbContext context)
        {
            if (await context.Promotions.AnyAsync()) return;

            await context.Promotions.AddRangeAsync(
                new Promotion
                {
                    Id = Guid.NewGuid(),
                    Title = "Weekend Special",
                    Description = "Get 20% off on all pizzas this weekend!",
                    DiscountPercent = 20,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30),
                    ImageUrl = "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=800",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Promotion
                {
                    Id = Guid.NewGuid(),
                    Title = "New User Offer",
                    Description = "First order? Enjoy 15% off on " +
                                      "your entire cart!",
                    DiscountPercent = 15,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(60),
                    ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=800",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Promotion
                {
                    Id = Guid.NewGuid(),
                    Title = "Lunch Hours Deal",
                    Description = "Order between 12–3 PM and get " +
                                      "flat 10% off!",
                    DiscountPercent = 10,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(45),
                    ImageUrl = "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=800",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Promotion
                {
                    Id = Guid.NewGuid(),
                    Title = "Loyalty Rewards",
                    Description = "Earn 1 point per ₹1 spent. " +
                                      "Redeem 10 points for ₹1 off!",
                    DiscountPercent = 0,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(365),
                    ImageUrl = "https://images.unsplash.com/photo-1607082348824-0a96f2a4b9da?w=800",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();
        }

        // ── Coupons ───────────────────────────────────────
        private static async Task SeedCouponsAsync(AppDbContext context)
        {
            if (await context.Coupons.AnyAsync()) return;

            await context.Coupons.AddRangeAsync(
                new Coupon
                {
                    Id = Guid.NewGuid(),
                    Code = "WELCOME20",
                    Description = "20% off on your first order",
                    DiscountPercent = 20,
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    UsageLimit = 1000,
                    MinOrderAmount = 199,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Coupon
                {
                    Id = Guid.NewGuid(),
                    Code = "URBAN10",
                    Description = "Flat 10% off on orders above ₹299",
                    DiscountPercent = 10,
                    ExpiryDate = DateTime.UtcNow.AddDays(60),
                    UsageLimit = 500,
                    MinOrderAmount = 299,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Coupon
                {
                    Id = Guid.NewGuid(),
                    Code = "BITES15",
                    Description = "15% off on orders above ₹499",
                    DiscountPercent = 15,
                    ExpiryDate = DateTime.UtcNow.AddDays(45),
                    UsageLimit = 200,
                    MinOrderAmount = 499,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Coupon
                {
                    Id = Guid.NewGuid(),
                    Code = "KFC20",
                    Description = "20% off on KFC orders above ₹399",
                    DiscountPercent = 20,
                    ExpiryDate = DateTime.UtcNow.AddDays(30),
                    UsageLimit = 300,
                    MinOrderAmount = 399,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Coupon
                {
                    Id = Guid.NewGuid(),
                    Code = "SUBWAY25",
                    Description = "25% off on Subway orders above ₹349",
                    DiscountPercent = 25,
                    ExpiryDate = DateTime.UtcNow.AddDays(30),
                    UsageLimit = 150,
                    MinOrderAmount = 349,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();
        }
    }
}