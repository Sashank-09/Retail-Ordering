using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UrbanBites.API.Extensions;
using UrbanBites.API.Middleware;
using UrbanBites.Application;
using UrbanBites.Application.Interfaces.Services;
using UrbanBites.Application.Services;
using UrbanBites.Infrastructure;
using UrbanBites.Infrastructure.Data;
using UrbanBites.Infrastructure.Identity;
using UrbanBites.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Layer Registrations ───────────────────────────────
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// ── Identity ──────────────────────────────────────────
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ── Auth Service ──────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();

// ── JWT ───────────────────────────────────────────────
var jwtKey = builder.Configuration["JwtSettings:Key"]!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
                                       Encoding.UTF8.GetBytes(jwtKey))
    };
});

// ── Swagger ───────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UrbanBites API",
        Version = "v1",
        Description = "Retail Ordering API"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token here."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ── CORS ──────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ── Rate Limiting ─────────────────────────────────────
builder.Services.AddRateLimitingPolicies();

// ── Render PORT binding ───────────────────────────────
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://*:{port}");

var app = builder.Build();

// ── Seed Database on Startup ──────────────────────────
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
                          .GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider
                          .GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider
                          .GetRequiredService<RoleManager<AppRole>>();

    await DbSeeder.SeedAsync(context, userManager, roleManager);
}

// ── Middleware Pipeline ───────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "UrbanBites API v1");
    options.RoutePrefix = "swagger";
});

// app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();