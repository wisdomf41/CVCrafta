using MailKit.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ResumeApp.Server.Configuration;

public sealed class SmtpEmailOptions
{
    public const string SectionName = "EmailDelivery:Smtp";

    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }

    public SecureSocketOptions Security { get; set; } =
        SecureSocketOptions.StartTls;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string SenderAddress { get; set; } = string.Empty;

    public string SenderName { get; set; } = string.Empty;
}

// Rejects incomplete SMTP settings and optional encryption in Production.
public sealed class SmtpEmailOptionsValidator :
    IValidateOptions<SmtpEmailOptions>
{
    private readonly IHostEnvironment _environment;

    public SmtpEmailOptionsValidator(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public ValidateOptionsResult Validate(
        string? name,
        SmtpEmailOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Host))
        {
            failures.Add("EmailDelivery:Smtp:Host is required.");
        }

        if (options.Port is < 1 or > 65535)
        {
            failures.Add(
                "EmailDelivery:Smtp:Port must be between 1 and 65535.");
        }

        if (!Enum.IsDefined(options.Security))
        {
            failures.Add("EmailDelivery:Smtp:Security is invalid.");
        }
        else if (_environment.IsProduction() &&
                 options.Security is not SecureSocketOptions.StartTls and
                     not SecureSocketOptions.SslOnConnect)
        {
            failures.Add(
                "EmailDelivery:Smtp:Security must be StartTls or " +
                "SslOnConnect in Production.");
        }

        if (string.IsNullOrWhiteSpace(options.Username))
        {
            failures.Add("EmailDelivery:Smtp:Username is required.");
        }

        if (string.IsNullOrWhiteSpace(options.Password))
        {
            failures.Add("EmailDelivery:Smtp:Password is required.");
        }

        if (!MailboxAddress.TryParse(options.SenderAddress, out _))
        {
            failures.Add(
                "EmailDelivery:Smtp:SenderAddress must be a valid email address.");
        }

        if (string.IsNullOrWhiteSpace(options.SenderName))
        {
            failures.Add("EmailDelivery:Smtp:SenderName is required.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
