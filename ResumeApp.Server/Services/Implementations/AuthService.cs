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
        private const string InvalidConfirmationMessage =
            "Invalid or expired verification link.";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailVerificationSender _emailVerificationSender;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IEmailVerificationSender emailVerificationSender,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailVerificationSender = emailVerificationSender;
            _logger = logger;
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

            var confirmationToken =
                await _userManager.GenerateEmailConfirmationTokenAsync(user);

            try
            {
                await _emailVerificationSender.SendVerificationLinkAsync(
                    user,
                    confirmationToken);
            }
            catch (EmailDeliveryException)
            {
                _logger.LogWarning(
                    "Registration completed but verification delivery failed.");

                return (
                    true,
                    "Registration successful. If the confirmation email " +
                    "does not arrive, request a new one.");
            }

            return (true, "Registration successful.");
        }

        public async Task<(AuthResponseDto? Response, string Message)> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return (null, "Invalid email or password.");
            }

            var validPassword =
                await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!validPassword)
            {
                return (null, "Invalid email or password.");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return (
                    null,
                    "Please confirm your email before logging in.");
            }

            return (CreateAuthResponse(user), string.Empty);
        }

        public async Task<(bool Success, string Message)> ConfirmEmailAsync(
            string userId,
            string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return (false, InvalidConfirmationMessage);
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return (true, "Email has already been confirmed.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return (false, InvalidConfirmationMessage);
            }

            return (true, "Email confirmed successfully.");
        }

        // Confirms an unused link before creating the normal login response.
        public async Task<(AuthResponseDto? Response, string Message)>
            ConfirmEmailAndLoginAsync(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(token))
            {
                return (null, InvalidConfirmationMessage);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null ||
                await _userManager.IsEmailConfirmedAsync(user))
            {
                return (null, InvalidConfirmationMessage);
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return (null, InvalidConfirmationMessage);
            }

            return (CreateAuthResponse(user), string.Empty);
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

            try
            {
                await _emailVerificationSender.SendVerificationLinkAsync(
                    user,
                    confirmationToken);
            }
            catch (EmailDeliveryException)
            {
                _logger.LogWarning(
                    "Email confirmation resend delivery failed.");
            }

            return (true, genericMessage);
        }

        private AuthResponseDto CreateAuthResponse(ApplicationUser user)
        {
            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user),
                Email = user.Email ?? string.Empty,
                FullName = user.FullName
            };
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
