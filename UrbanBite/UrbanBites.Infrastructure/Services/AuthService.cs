using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UrbanBites.Application.DTOs.Auth;
using UrbanBites.Application.Interfaces.Services;
using UrbanBites.Infrastructure.Identity;
using UrbanBites.Infrastructure.Services;

namespace UrbanBites.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;


        public AuthService(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IConfiguration configuration,
            IOtpService otpService,
            IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _otpService = otpService;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser is not null)
                throw new Exception("Email is already registered.");

            var user = new AppUser
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email,
                PhoneNumber = dto.Phone
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ",
                    result.Errors.Select(e => e.Description));
                throw new Exception($"Registration failed: {errors}");
            }

            if (!await _roleManager.RoleExistsAsync("Customer"))
                await _roleManager.CreateAsync(new AppRole { Name = "Customer" });

            await _userManager.AddToRoleAsync(user, "Customer");

            // ── Send Welcome Email ────────────────────────────
            try
            {
                await _emailService.SendWelcomeEmailAsync(
                    user.Email!, user.FullName);
            }
            catch { /* Email failure never blocks registration */ }

            return await GenerateAuthResponse(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            // Find user
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null || user.IsDeleted)
                throw new Exception("Invalid email or password.");

            // Verify password
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
                throw new Exception("Invalid email or password.");

            return await GenerateAuthResponse(user);
        }

        // ── Private Helper ────────────────────────────────
        private async Task<AuthResponseDto> GenerateAuthResponse(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Customer";
            var token = GenerateJwtToken(user, role);

            return new AuthResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email!,
                Role = role,
                ExpiresAt = DateTime.UtcNow.AddDays(
                    int.Parse(_configuration["JwtSettings:ExpiryInDays"]!))
            };
        }

        private string GenerateJwtToken(AppUser user, string role)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(
                    int.Parse(_configuration["JwtSettings:ExpiryInDays"]!)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                throw new Exception("No account found with this email.");

            await _otpService.SendOtpAsync(email);
        }

        public async Task<bool> VerifyOtpAsync(string email, string otpCode)
            => await _otpService.VerifyOtpAsync(email, otpCode, markAsUsed: false);

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            // Verify OTP first AND mark as used
            var isValid = await _otpService.VerifyOtpAsync(dto.Email, dto.OtpCode, markAsUsed: true);
            if (!isValid) throw new Exception("Invalid or expired OTP.");

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null) throw new Exception("User not found.");

            // Reset password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Password reset failed: {errors}");
            }

            return true;
        }

        public async Task<AuthResponseDto> GoogleSignInAsync(string googleToken)
        {
            // Verify Google token
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(
                $"https://oauth2.googleapis.com/tokeninfo?id_token={googleToken}");

            var payload = System.Text.Json.JsonSerializer.Deserialize
                <GoogleTokenPayload>(response);

            if (payload is null || string.IsNullOrEmpty(payload.Email))
                throw new Exception("Invalid Google token.");

            // Find or create user
            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user is null)
            {
                user = new AppUser
                {
                    FullName = payload.Name ?? payload.Email,
                    Email = payload.Email,
                    UserName = payload.Email
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    throw new Exception("Failed to create Google account.");

                if (!await _roleManager.RoleExistsAsync("Customer"))
                    await _roleManager.CreateAsync(new AppRole { Name = "Customer" });

                await _userManager.AddToRoleAsync(user, "Customer");
            }

            return await GenerateAuthResponse(user);
        }

        private class GoogleTokenPayload
        {
            [System.Text.Json.Serialization.JsonPropertyName("email")]
            public string Email { get; set; } = string.Empty;

            [System.Text.Json.Serialization.JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
        }
    }
}