using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using ResumeApp.Server.ApplicationUserModel;
using ResumeApp.Server.Configuration;
using ResumeApp.Server.Extensions;
using ResumeApp.Server.Services.Implementations;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Tests;

// Verifies sender selection, TLS validation, message content, and safe failures.
public class EmailVerificationDeliveryTests
{
    [Fact]
    public void AddEmailVerificationDelivery_SelectsDevelopmentSender()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(new()
        {
            ["EmailDelivery:Provider"] = "Development"
        });

        services.AddEmailVerificationDelivery(
            configuration,
            new TestHostEnvironment("Development"));

        Assert.Contains(
            services,
            descriptor =>
                descriptor.ServiceType == typeof(IEmailVerificationSender) &&
                descriptor.ImplementationType ==
                    typeof(DevelopmentEmailVerificationSender));
    }

    [Fact]
    public void AddEmailVerificationDelivery_SelectsSmtpSender()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(new()
        {
            ["EmailDelivery:Provider"] = "Smtp",
            ["App:PublicBaseUrl"] = "https://resume.example.test"
        });

        services.AddEmailVerificationDelivery(
            configuration,
            new TestHostEnvironment("Production"));

        Assert.Contains(
            services,
            descriptor =>
                descriptor.ServiceType == typeof(IEmailVerificationSender) &&
                descriptor.ImplementationType ==
                    typeof(SmtpEmailVerificationSender));

        Assert.Contains(
            services,
            descriptor =>
                descriptor.ServiceType == typeof(ISmtpEmailClientFactory) &&
                descriptor.ImplementationType ==
                    typeof(MailKitSmtpEmailClientFactory));
    }

    [Fact]
    public void AddEmailVerificationDelivery_RequiresPublicBaseUrlForSmtp()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(new()
        {
            ["EmailDelivery:Provider"] = "Smtp"
        });

        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.AddEmailVerificationDelivery(
                configuration,
                new TestHostEnvironment("Production")));

        Assert.Contains("App:PublicBaseUrl", exception.Message);
    }

    [Fact]
    public void AddEmailVerificationDelivery_RejectsDevelopmentInProduction()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(new()
        {
            ["EmailDelivery:Provider"] = "Development"
        });

        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.AddEmailVerificationDelivery(
                configuration,
                new TestHostEnvironment("Production")));

        Assert.Contains(
            "cannot be Development",
            exception.Message);
    }

    [Fact]
    public void AddEmailVerificationDelivery_RequiresExplicitProvider()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration([]);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.AddEmailVerificationDelivery(
                configuration,
                new TestHostEnvironment("Development")));

        Assert.Contains(
            "must be explicitly configured",
            exception.Message);
    }

    [Fact]
    public void SmtpOptionsValidator_ReportsOnlyConfigurationNames()
    {
        var validator = new SmtpEmailOptionsValidator(
            new TestHostEnvironment("Production"));

        var result = validator.Validate(null, new SmtpEmailOptions());

        Assert.True(result.Failed);
        Assert.Contains(
            "EmailDelivery:Smtp:Host is required.",
            result.Failures);
        Assert.Contains(
            "EmailDelivery:Smtp:Password is required.",
            result.Failures);
        Assert.DoesNotContain(
            result.Failures,
            failure => failure.Contains('='));
    }

    [Fact]
    public void SmtpOptionsValidator_AcceptsCompleteConfiguration()
    {
        var validator = new SmtpEmailOptionsValidator(
            new TestHostEnvironment("Production"));

        var result = validator.Validate(null, CreateValidOptions());

        Assert.True(result.Succeeded);
    }

    [Theory]
    [InlineData(SecureSocketOptions.None)]
    [InlineData(SecureSocketOptions.Auto)]
    [InlineData(SecureSocketOptions.StartTlsWhenAvailable)]
    public void SmtpOptionsValidator_RejectsOptionalTlsInProduction(
        SecureSocketOptions security)
    {
        var validator = new SmtpEmailOptionsValidator(
            new TestHostEnvironment("Production"));
        var options = CreateValidOptions();
        options.Security = security;

        var result = validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(
            "EmailDelivery:Smtp:Security must be StartTls or " +
            "SslOnConnect in Production.",
            result.Failures);
    }

    [Theory]
    [InlineData(SecureSocketOptions.StartTls)]
    [InlineData(SecureSocketOptions.SslOnConnect)]
    public void SmtpOptionsValidator_AcceptsRequiredTlsInProduction(
        SecureSocketOptions security)
    {
        var validator = new SmtpEmailOptionsValidator(
            new TestHostEnvironment("Production"));
        var options = CreateValidOptions();
        options.Security = security;

        var result = validator.Validate(null, options);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public void SmtpOptionsValidator_AllowsLocalRelayWithoutTlsInDevelopment()
    {
        var validator = new SmtpEmailOptionsValidator(
            new TestHostEnvironment("Development"));
        var options = CreateValidOptions();
        options.Security = SecureSocketOptions.None;

        var result = validator.Validate(null, options);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task SmtpSender_SendsMultipartConfirmationMessage()
    {
        var client = new RecordingSmtpEmailClient();
        var logger = new RecordingLogger<SmtpEmailVerificationSender>();
        var sender = CreateSmtpSender(client, logger);
        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "recipient@example.test",
            FullName = "Test Recipient"
        };

        await sender.SendVerificationLinkAsync(
            user,
            "token-with+reserved/value");

        Assert.Equal("smtp.example.test", client.Host);
        Assert.Equal(587, client.Port);
        Assert.Equal(SecureSocketOptions.StartTls, client.Security);
        Assert.True(client.Authenticated);
        Assert.True(client.Disconnected);

        var message = Assert.IsType<MimeMessage>(client.Message);

        Assert.Equal(
            "Confirm your ResumeApp account",
            message.Subject);
        Assert.Contains("ResumeApp", message.TextBody);
        Assert.Contains("expires in 2 hours", message.TextBody);
        Assert.Contains(
            "token=token-with%2Breserved%2Fvalue",
            message.TextBody);
        Assert.Contains(
            "https://resume.example.test/confirm-email#",
            message.TextBody);
        Assert.DoesNotContain(
            "/api/Auth/confirm-email",
            message.TextBody);
        Assert.Contains("Confirm email", message.HtmlBody);
        Assert.Contains("Do not share or forward it", message.HtmlBody);
        Assert.Empty(logger.Messages);
    }

    [Fact]
    public async Task SmtpSender_SanitizesProviderFailureAndLogs()
    {
        const string sensitiveDetail = "test-only-provider-detail";
        const string confirmationToken = "test-only-confirmation-token";

        var client = new RecordingSmtpEmailClient
        {
            SendException = new InvalidOperationException(sensitiveDetail)
        };
        var logger = new RecordingLogger<SmtpEmailVerificationSender>();
        var sender = CreateSmtpSender(client, logger);
        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "recipient@example.test",
            FullName = "Test Recipient"
        };

        var exception = await Assert.ThrowsAsync<EmailDeliveryException>(() =>
            sender.SendVerificationLinkAsync(user, confirmationToken));

        Assert.Equal(
            "Email verification delivery failed.",
            exception.Message);
        Assert.Null(exception.InnerException);

        var logOutput = string.Join(Environment.NewLine, logger.Messages);

        Assert.DoesNotContain(sensitiveDetail, logOutput);
        Assert.DoesNotContain(confirmationToken, logOutput);
        Assert.DoesNotContain(CreateValidOptions().Password, logOutput);
        Assert.DoesNotContain("recipient@example.test", logOutput);
        Assert.DoesNotContain("https://resume.example.test", logOutput);
        Assert.Contains(
            nameof(InvalidOperationException),
            logOutput);
    }

    private static SmtpEmailVerificationSender CreateSmtpSender(
        RecordingSmtpEmailClient client,
        ILogger<SmtpEmailVerificationSender> logger)
    {
        var configuration = CreateConfiguration(new()
        {
            ["App:PublicBaseUrl"] = "https://resume.example.test"
        });

        return new SmtpEmailVerificationSender(
            Options.Create(CreateValidOptions()),
            configuration,
            new RecordingSmtpEmailClientFactory(client),
            logger);
    }

    private static SmtpEmailOptions CreateValidOptions()
    {
        return new SmtpEmailOptions
        {
            Host = "smtp.example.test",
            Port = 587,
            Security = SecureSocketOptions.StartTls,
            Username = "test-user",
            Password = "test-only-value",
            SenderAddress = "no-reply@example.test",
            SenderName = "ResumeApp"
        };
    }

    private static IConfiguration CreateConfiguration(
        Dictionary<string, string?> values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }

    private sealed class RecordingSmtpEmailClientFactory :
        ISmtpEmailClientFactory
    {
        private readonly ISmtpEmailClient _client;

        public RecordingSmtpEmailClientFactory(ISmtpEmailClient client)
        {
            _client = client;
        }

        public ISmtpEmailClient Create()
        {
            return _client;
        }
    }

    private sealed class RecordingSmtpEmailClient : ISmtpEmailClient
    {
        public string Host { get; private set; } = string.Empty;

        public int Port { get; private set; }

        public SecureSocketOptions Security { get; private set; }

        public bool Authenticated { get; private set; }

        public bool Disconnected { get; private set; }

        public MimeMessage? Message { get; private set; }

        public Exception? SendException { get; init; }

        public Task ConnectAsync(
            string host,
            int port,
            SecureSocketOptions security,
            CancellationToken cancellationToken)
        {
            Host = host;
            Port = port;
            Security = security;

            return Task.CompletedTask;
        }

        public Task AuthenticateAsync(
            string username,
            string password,
            CancellationToken cancellationToken)
        {
            Authenticated = true;

            return Task.CompletedTask;
        }

        public Task SendAsync(
            MimeMessage message,
            CancellationToken cancellationToken)
        {
            if (SendException != null)
            {
                return Task.FromException(SendException);
            }

            Message = message;

            return Task.CompletedTask;
        }

        public Task DisconnectAsync(
            bool quit,
            CancellationToken cancellationToken)
        {
            Disconnected = quit;

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }

    private sealed class RecordingLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = [];

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public TestHostEnvironment(string environmentName)
        {
            EnvironmentName = environmentName;
        }

        public string EnvironmentName { get; set; }

        public string ApplicationName { get; set; } = "ResumeApp.Tests";

        public string ContentRootPath { get; set; } = string.Empty;

        public IFileProvider ContentRootFileProvider { get; set; } =
            new NullFileProvider();
    }
}
