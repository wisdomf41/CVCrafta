using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Services.Implementations;

// Adapts MailKit's SMTP client behind the delivery test seam.
public sealed class MailKitSmtpEmailClientFactory :
    ISmtpEmailClientFactory
{
    public ISmtpEmailClient Create()
    {
        return new MailKitSmtpEmailClient(new SmtpClient());
    }
}

internal sealed class MailKitSmtpEmailClient : ISmtpEmailClient
{
    private readonly SmtpClient _client;

    public MailKitSmtpEmailClient(SmtpClient client)
    {
        _client = client;
    }

    public Task ConnectAsync(
        string host,
        int port,
        SecureSocketOptions security,
        CancellationToken cancellationToken)
    {
        return _client.ConnectAsync(
            host,
            port,
            security,
            cancellationToken);
    }

    public Task AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        return _client.AuthenticateAsync(
            username,
            password,
            cancellationToken);
    }

    public async Task SendAsync(
        MimeMessage message,
        CancellationToken cancellationToken)
    {
        await _client.SendAsync(message, cancellationToken);
    }

    public Task DisconnectAsync(
        bool quit,
        CancellationToken cancellationToken)
    {
        return _client.DisconnectAsync(quit, cancellationToken);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
