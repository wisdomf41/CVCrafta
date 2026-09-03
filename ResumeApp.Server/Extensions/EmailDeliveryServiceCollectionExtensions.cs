using Microsoft.Extensions.Options;
using ResumeApp.Server.Configuration;
using ResumeApp.Server.Services.Implementations;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Extensions;

// Selects an explicitly configured sender and blocks development logging in production.
public static class EmailDeliveryServiceCollectionExtensions
{
    public static IServiceCollection AddEmailVerificationDelivery(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var provider = configuration["EmailDelivery:Provider"]?.Trim();

        if (string.Equals(
                provider,
                "Development",
                StringComparison.OrdinalIgnoreCase))
        {
            if (!environment.IsDevelopment() &&
                !environment.IsEnvironment("Testing"))
            {
                throw new InvalidOperationException(
                    "EmailDelivery:Provider cannot be Development outside " +
                    "the Development or Testing environment.");
            }

            services.AddScoped<
                IEmailVerificationSender,
                DevelopmentEmailVerificationSender>();

            return services;
        }

        if (string.Equals(
                provider,
                "Smtp",
                StringComparison.OrdinalIgnoreCase))
        {
            ValidatePublicBaseUrl(configuration);

            services.AddSingleton<
                IValidateOptions<SmtpEmailOptions>,
                SmtpEmailOptionsValidator>();

            services.AddOptions<SmtpEmailOptions>()
                .Bind(configuration.GetSection(
                    SmtpEmailOptions.SectionName))
                .ValidateOnStart();

            services.AddSingleton<
                ISmtpEmailClientFactory,
                MailKitSmtpEmailClientFactory>();

            services.AddScoped<
                IEmailVerificationSender,
                SmtpEmailVerificationSender>();

            return services;
        }

        throw new InvalidOperationException(
            "EmailDelivery:Provider must be explicitly configured as " +
            "Development or Smtp.");
    }

    private static void ValidatePublicBaseUrl(
        IConfiguration configuration)
    {
        var configuredUrl = configuration["App:PublicBaseUrl"];

        if (!Uri.TryCreate(
                configuredUrl,
                UriKind.Absolute,
                out var publicBaseUrl) ||
            (publicBaseUrl.Scheme != Uri.UriSchemeHttp &&
             publicBaseUrl.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException(
                "SMTP email delivery configuration is invalid. " +
                "App:PublicBaseUrl must be an absolute HTTP or HTTPS URL.");
        }
    }
}
