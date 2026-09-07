using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using ResumeApp.Server.ApplicationUserModel;
using ResumeApp.Server.Configuration;
using ResumeApp.Server.Services.Interfaces;
using System.Text.Encodings.Web;

namespace ResumeApp.Server.Services.Implementations;

// Sends multipart email linking to the frontend confirmation processor.
public sealed class SmtpEmailVerificationSender :
    IEmailVerificationSender
{
    private const string SafeFailureMessage =
        "Email verification delivery failed.";

    private readonly SmtpEmailOptions _options;
    private readonly IConfiguration _configuration;
    private readonly ISmtpEmailClientFactory _clientFactory;
    private readonly ILogger<SmtpEmailVerificationSender> _logger;

    public SmtpEmailVerificationSender(
        IOptions<SmtpEmailOptions> options,
        IConfiguration configuration,
        ISmtpEmailClientFactory clientFactory,
        ILogger<SmtpEmailVerificationSender> logger)
    {
        _options = options.Value;
        _configuration = configuration;
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public async Task SendVerificationLinkAsync(
        ApplicationUser user,
        string token,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new EmailDeliveryException(SafeFailureMessage);
        }

        var verificationLink = BuildVerificationLink(user.Id, token);
        var message = BuildMessage(user, verificationLink);

        try
        {
            using var client = _clientFactory.Create();

            await client.ConnectAsync(
                _options.Host,
                _options.Port,
                _options.Security,
                cancellationToken);

            await client.AuthenticateAsync(
                _options.Username,
                _options.Password,
                cancellationToken);

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                "SMTP email verification delivery failed ({FailureType}).",
                exception.GetType().Name);

            throw new EmailDeliveryException(SafeFailureMessage);
        }
    }

    private string BuildVerificationLink(string userId, string token)
    {
        var publicBaseUrl =
            _configuration["App:PublicBaseUrl"]!.TrimEnd('/');

        return $"{publicBaseUrl}/confirm-email" +
               $"#userId={Uri.EscapeDataString(userId)}" +
               $"&token={Uri.EscapeDataString(token)}";
    }

    private MimeMessage BuildMessage(
        ApplicationUser user,
        string verificationLink)
    {
        var displayName = string.IsNullOrWhiteSpace(user.FullName)
            ? "there"
            : user.FullName.Trim();
        var encodedName = HtmlEncoder.Default.Encode(displayName);
        var encodedLink = HtmlEncoder.Default.Encode(verificationLink);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            _options.SenderName,
            _options.SenderAddress));
        message.To.Add(new MailboxAddress(displayName, user.Email!));
        message.Subject = "Confirm your ResumeApp account";

        var body = new BodyBuilder
        {
            TextBody =
                $"Hello {displayName},{Environment.NewLine}{Environment.NewLine}" +
                "Welcome to ResumeApp. Confirm your account by opening this link:" +
                $"{Environment.NewLine}{verificationLink}" +
                $"{Environment.NewLine}{Environment.NewLine}" +
                "This confirmation link expires in 2 hours. " +
                "Do not share or forward it. If you did not create this account, " +
                "you can safely ignore this email.",
            HtmlBody =
                "<!doctype html><html><body style=\"font-family:Arial,sans-serif;" +
                "color:#0f172a;line-height:1.6\">" +
                $"<p>Hello {encodedName},</p>" +
                "<p>Welcome to <strong>ResumeApp</strong>. Confirm your account " +
                "to finish setting up your resume workspace.</p>" +
                $"<p><a href=\"{encodedLink}\" style=\"display:inline-block;" +
                "background:#2563eb;color:#ffffff;padding:12px 20px;" +
                "border-radius:8px;text-decoration:none;font-weight:700\">" +
                "Confirm email</a></p>" +
                $"<p>If the button does not work, open this link:<br>" +
                $"<a href=\"{encodedLink}\">{encodedLink}</a></p>" +
                "<p>This confirmation link expires in 2 hours. Do not share or " +
                "forward it. If you did not create this account, you can safely " +
                "ignore this email.</p></body></html>"
        };

        message.Body = body.ToMessageBody();

        return message;
    }
}

public sealed class EmailDeliveryException : Exception
{
    public EmailDeliveryException(string message)
        : base(message)
    {
    }
}
