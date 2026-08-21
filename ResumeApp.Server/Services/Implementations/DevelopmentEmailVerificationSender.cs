using ResumeApp.Server.ApplicationUserModel;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Services.Implementations
{
    // Added to log local verification links without requiring an email provider.
    public class DevelopmentEmailVerificationSender : IEmailVerificationSender
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;
        private readonly ILogger<DevelopmentEmailVerificationSender> _logger;

        public DevelopmentEmailVerificationSender(
            IConfiguration configuration,
            IHostEnvironment environment,
            ILogger<DevelopmentEmailVerificationSender> logger)
        {
            _configuration = configuration;
            _environment = environment;
            _logger = logger;
        }

        public Task SendVerificationLinkAsync(
            ApplicationUser user,
            string token,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_environment.IsDevelopment() &&
                !_environment.IsEnvironment("Testing"))
            {
                throw new InvalidOperationException(
                    "A production email-verification sender has not been configured.");
            }

            var publicBaseUrl =
                (_configuration["App:PublicBaseUrl"] ?? "http://localhost:8080")
                .TrimEnd('/');

            var verificationLink =
                $"{publicBaseUrl}/api/Auth/confirm-email" +
                $"?userId={Uri.EscapeDataString(user.Id)}" +
                $"&token={Uri.EscapeDataString(token)}";

            _logger.LogInformation(
                "Development verification link for {Email}: {VerificationLink}",
                user.Email,
                verificationLink);

            return Task.CompletedTask;
        }
    }
}
