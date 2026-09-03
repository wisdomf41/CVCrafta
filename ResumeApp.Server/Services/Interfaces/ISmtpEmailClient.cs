using MailKit.Security;
using MimeKit;

namespace ResumeApp.Server.Services.Interfaces;

// Abstracts MailKit network operations so automated tests never contact SMTP.
public interface ISmtpEmailClient : IDisposable
{
    Task ConnectAsync(
        string host,
        int port,
        SecureSocketOptions security,
        CancellationToken cancellationToken);

    Task AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken);

    Task SendAsync(
        MimeMessage message,
        CancellationToken cancellationToken);

    Task DisconnectAsync(
        bool quit,
        CancellationToken cancellationToken);
}

public interface ISmtpEmailClientFactory
{
    ISmtpEmailClient Create();
}
