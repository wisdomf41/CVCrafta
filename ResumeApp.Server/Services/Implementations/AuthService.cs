using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ResumeApp.Server.ApplicationUserModel;
using ResumeApp.Server.DTOs.Auth;
using ResumeApp.Server.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ResumeApp.Server.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailVerificationSender _emailVerificationSender;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IEmailVerificationSender emailVerificationSender)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailVerificationSender = emailVerificationSender;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(
            RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                return (false, "User already exists.");
            }

            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    " | ",
                    result.Errors.Select(error => error.Description));

                return (false, errors);
            }

            // Added secure email-confirmation token generation.
            var confirmationToken =
                await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _emailVerificationSender.SendVerificationLinkAsync(
                user,
                confirmationToken);

            return (true, "Registration successful.");
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return null;
            }

            var validPassword =
                await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!validPassword)
            {
                return null;
            }

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName
            };
        }

        public async Task<(bool Success, string Message)> ConfirmEmailAsync(
            string userId,
            string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return (false, "Invalid or expired verification link.");
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return (true, "Email has already been confirmed.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return (false, "Invalid or expired verification link.");
            }

            return (true, "Email confirmed successfully.");
        }

        public async Task<(bool Success, string Message)>
            ResendEmailConfirmationAsync(string email)
        {
            var genericMessage =
                "If an unverified account exists, a new verification link has been generated.";

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null ||
                await _userManager.IsEmailConfirmedAsync(user))
            {
                return (true, genericMessage);
            }

            var confirmationToken =
                await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _emailVerificationSender.SendVerificationLinkAsync(
                user,
                confirmationToken);

            return (true, genericMessage);
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT signing key is missing.");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}