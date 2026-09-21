using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using ResumeApp.Server.ApplicationUserModel;
using ResumeApp.Server.Services.Implementations;
using ResumeApp.Server.Services.Interfaces;
using ResumeApp.Server.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using ResumeApp.Server.DTOs.Auth;

namespace ResumeApp.Server.Tests.Integration;

// Covers token expiry, confirmation, resend, and recoverable delivery failures.
public class EmailVerificationApiIntegrationTests :
    IClassFixture<CustomWebApplicationFactory>
{
    private const string ValidPassword = "Test@12345";

    private readonly CustomWebApplicationFactory _factory;

    public EmailVerificationApiIntegrationTests(
        CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public void EmailConfirmationTokenLifetime_IsExactlyTwoHours()
    {
        var sender = new RecordingEmailVerificationSender();

        using var application = CreateApplication(sender);

        var options = application.Services
            .GetRequiredService<IOptions<DataProtectionTokenProviderOptions>>()
            .Value;

        Assert.Equal(TimeSpan.FromHours(2), options.TokenLifespan);
    }

    [Fact]
    public async Task ConfirmEmail_WithValidToken_ConfirmsUser()
    {
        var sender = new RecordingEmailVerificationSender();

        using var application = CreateApplication(sender);
        using var client = application.CreateClient();

        var email = CreateUniqueEmail("confirm-valid");

        await RegisterAsync(client, email);

        var verification = Assert.Single(
            sender.Messages, message => message.Email == email);

        using var response = await client.GetAsync(
            CreateConfirmationPath(verification));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Contains("Email confirmed successfully.", responseContent);

        var user = await GetUserAsync(application, email);

        using var scope = application.Services.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        Assert.True(await userManager.IsEmailConfirmedAsync(user));
    }

    [Fact]
    public async Task ConfirmEmail_WithInvalidToken_ReturnsBadRequest()
    {
        var sender = new RecordingEmailVerificationSender();

        using var application = CreateApplication(sender);
        using var client = application.CreateClient();

        var email = CreateUniqueEmail("confirm-invalid");

        await RegisterAsync(client, email);

        var verification = Assert.Single(
            sender.Messages, message => message.Email == email);

        var invalidVerification = verification with
        {
            Token = "invalid-confirmation-token"
        };

        using var response = await client.GetAsync(
            CreateConfirmationPath(invalidVerification));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "Invalid or expired verification link.",
            responseContent);

        var user = await GetUserAsync(application, email);

        using var scope = application.Services.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        Assert.False(await userManager.IsEmailConfirmedAsync(user));
    }

    [Fact]
    public async Task ConfirmEmailAndLogin_WithValidToken_ReturnsAuthentication()
    {
        var sender = new RecordingEmailVerificationSender();

        using var application = CreateApplication(sender);
        using var client = application.CreateClient();

        var email = CreateUniqueEmail("confirm-login-valid");

        await RegisterAsync(client, email);

        var verification = Assert.Single(
            sender.Messages, message => message.Email == email);

        using var response = await ConfirmEmailAndLoginAsync(
            client,
            verification);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var authResponse =
            await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        Assert.NotNull(authResponse);
        Assert.Equal(email, authResponse.Email);
        Assert.Equal("Email Verification Test", authResponse.FullName);
        Assert.Equal(3, authResponse.Token.Split('.').Length);

        var user = await GetUserAsync(application, email);

        using var scope = application.Services.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        Assert.True(await userManager.IsEmailConfirmedAsync(user));
    }

    [Fact]
    public async Task ConfirmEmailAndLogin_WithInvalidToken_DoesNotAuthenticate()
    {
        var sender = new RecordingEmailVerificationSender();

        using var application = CreateApplication(sender);
        using var client = application.CreateClient();

        var email = CreateUniqueEmail("confirm-login-invalid");

        await RegisterAsync(client, email);

        var verification = Assert.Single(
            sender.Messages, message => message.Email == email) with
        {
            Token = "invalid-confirmation-token"
        };

        using var response = await ConfirmEmailAndLoginAsync(
            client,
            verification);

        await AssertSafeConfirmationFailureAsync(response, verification);
    }

    [Fact]
    public async Task ConfirmEmailAndLogin_WithUnknownUser_UsesGenericFailure()
    {
        var sender = new RecordingEmailVerificationSender();

        using var application = CreateApplication(sender);
        using var client = application.CreateClient();
        var verification = new VerificationMessage(
            "unknown@example.com",
            "unknown-user-id",
            "unknown-confirmation-token");

        using var response = await ConfirmEmailAndLoginAsync(
            client,
            verification);

        await AssertSafeConfirmationFailureAsync(response, verification);
    }

    [Fact]
    public async Task ConfirmEmailAndLogin_WithExpiredToken_DoesNotAuthenticate()
    {
        var sender = new RecordingEmailVerificationSender();

        using var application = CreateApplication(
            sender,
            services => services.PostConfigure<
                DataProtectionTokenProviderOptions>(options =>
                    options.TokenLifespan = TimeSpan.FromTicks(-1)));
        using var client = application.CreateClient();

        var email = CreateUniqueEmail("confirm-login-expired");

        await RegisterAsync(client, email);

        var verification = Assert.Single(
            sender.Messages, message => message.Email == email);

        using var response = await ConfirmEmailAndLoginAsync(
            client,
            verification);

        await AssertSafeConfirmationFailureAsync(response, verification);
    }

    [Fact]
    public async Task ConfirmEmailAndLogin_WithMissingData_DoesNotAuthenticate()
    {
        var sender = new RecordingEmailVerificationSender();

        using var application = CreateApplication(sender);
        using var client = application.CreateClient();

        using var response = await client.PostAsJsonAsync(
            "/api/Auth/confirm-email",
            new
            {
                userId = "",
                token = ""
            });

        await AssertSafeConfirmationFailureAsync(response);
    }

    [Fact]
    public async Task ConfirmEmailAndLogin_WithMalformedJson_DoesNotAuthenticate()
    {
        var sender = new RecordingEmailVerificationSender();

        using var application = CreateApplication(sender);
        using var client = application.CreateClient();
        using var content = new StringContent(
            "{ malformed",
            Encoding.UTF8,
            "application/json");

        using var response = await client.PostAsync(
            "/api/Auth/confirm-email",
            content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.DoesNotContain("eyJ", responseContent);
    }

    [Fact]
    public async Task ConfirmEmailAndLogin_WhenReused_DoesNotIssueAnotherToken()
    {
        var sender = new RecordingEmailVerificationSender();

        using var application = CreateApplication(sender);
        using var client = application.CreateClient();

        var email = CreateUniqueEmail("confirm-login-reused");

        await RegisterAsync(client, email);

        var verification = Assert.Single(
            sender.Messages, message => message.Email == email);

        using var firstResponse = await ConfirmEmailAndLoginAsync(
            client,
            verification);

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        using var secondResponse = await ConfirmEmailAndLoginAsync(
            client,
            verification);

        await AssertSafeConfirmationFailureAsync(
            secondResponse,
            verification);
    }

    [Fact]
    public async Task ConfirmEmailAndLogin_WhenConcurrent_IssuesOnlyOneToken()
    {
        var sender = new RecordingEmailVerificationSender();

        using var application = CreateApplication(sender);
        using var client = application.CreateClient();

        var email = CreateUniqueEmail("confirm-login-concurrent");

        await RegisterAsync(client, email);

        var verification = Assert.Single(
            sender.Messages, message => message.Email == email);

        var responses = await Task.WhenAll(
            ConfirmEmailAndLoginAsync(client, verification),
            ConfirmEmailAndLoginAsync(client, verification));

        try
        {
            Assert.Single(
                responses,
                response => response.StatusCode == HttpStatusCode.OK);
            Assert.Single(
                responses,
                response => response.StatusCode == HttpStatusCode.BadRequest);

            var failedResponse = Assert.Single(
                responses,
                response => response.StatusCode == HttpStatusCode.BadRequest);

            await AssertSafeConfirmationFailureAsync(
                failedResponse,
                verification);
        }
        finally
        {
            foreach (var response in responses)
            {
                response.Dispose();
            }
        }
    }

    [Fact]
    public async Task ConfirmEmailAndLogin_WhenAlreadyConfirmed_DoesNotAuthenticate()
    {
        var sender = new RecordingEmailVerificationSender();

        using var application = CreateApplication(sender);
        using var client = application.CreateClient();

        var email = CreateUniqueEmail("confirm-login-already-confirmed");

        await RegisterAsync(client, email);

        var verification = Assert.Single(
            sender.Messages, message => message.Email == email);

        using var legacyResponse = await client.GetAsync(
            CreateConfirmationPath(verification));

        Assert.Equal(HttpStatusCode.OK, legacyResponse.StatusCode);

        using var response = await ConfirmEmailAndLoginAsync(
            client,
            verification);

        await AssertSafeConfirmationFailureAsync(response, verification);
    }

    [Fact]
    public async Task ConfirmEmail_WhenAlreadyConfirmed_ReturnsOk()
    {
        var sender = new RecordingEmailVerificationSender();

        using var application = CreateApplication(sender);
        using var client = application.CreateClient();

        var email = CreateUniqueEmail("confirm-twice");

        await RegisterAsync(client, email);

        var verification = Assert.Single(
            sender.Messages, message => message.Email == email);

        var confirmationPath = CreateConfirmationPath(verification);

        using var firstResponse = await client.GetAsync(confirmationPath);

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        using var secondResponse = await client.GetAsync(confirmationPath);

        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);

        var responseContent =
            await secondResponse.Content.ReadAsStringAsync();

        Assert.Contains(
            "Email has already been confirmed.",
            responseContent);
    }

    [Fact]
    public async Task ResendConfirmation_ForUnverifiedUser_GeneratesNewMessage()
    {
        var sender = new RecordingEmailVerificationSender();

        using var application = CreateApplication(sender);
        using var client = application.CreateClient();

        var email = CreateUniqueEmail("resend-unverified");

        await RegisterAsync(client, email);

        Assert.Single(
            sender.Messages, message => message.Email == email);

        using var response = await client.PostAsJsonAsync(
            "/api/Auth/resend-email-confirmation",
            new
            {
                email
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Equal(
            2,
            sender.Messages.Count(message => message.Email == email));

        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "If an unverified account exists",
            responseContent);
    }

    [Fact]
    public async Task ResendConfirmation_ForUnknownEmail_ReturnsSafeResponse()
    {
        var sender = new RecordingEmailVerificationSender();

        using var application = CreateApplication(sender);
        using var client = application.CreateClient();

        var email = CreateUniqueEmail("unknown");

        using var response = await client.PostAsJsonAsync(
            "/api/Auth/resend-email-confirmation",
            new
            {
                email
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(sender.Messages);

        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "If an unverified account exists",
            responseContent);
    }

    [Fact]
    public async Task Register_WhenDeliveryFails_RemainsRecoverable()
    {
        var sender = new FailOnFirstEmailVerificationSender();

        using var application = CreateApplication(sender);
        using var client = application.CreateClient();

        var email = CreateUniqueEmail("delivery-failure");

        using var response = await client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                fullName = "Delivery Failure Test",
                email,
                password = ValidPassword
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "Registration successful. If the confirmation email does not arrive, " +
            "request a new one.",
            responseContent);
        Assert.DoesNotContain(
            FailOnFirstEmailVerificationSender.SensitiveDetail,
            responseContent);
        Assert.DoesNotContain(sender.ConfirmationToken, responseContent);

        var user = await GetUserAsync(application, email);

        using var scope = application.Services.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        Assert.False(await userManager.IsEmailConfirmedAsync(user));

        using var resendResponse = await client.PostAsJsonAsync(
            "/api/Auth/resend-email-confirmation",
            new
            {
                email
            });

        Assert.Equal(HttpStatusCode.OK, resendResponse.StatusCode);
        Assert.Contains(
            "If an unverified account exists",
            await resendResponse.Content.ReadAsStringAsync());
        Assert.Equal(2, sender.SendCount);

        using var retryResponse = await client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                fullName = "Delivery Failure Test",
                email,
                password = ValidPassword
            });

        Assert.Equal(HttpStatusCode.BadRequest, retryResponse.StatusCode);
        Assert.Contains(
            "User already exists.",
            await retryResponse.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Resend_WhenDeliveryFails_PreservesGenericResponse()
    {
        var sender = new FailOnSecondEmailVerificationSender();

        using var application = CreateApplication(sender);
        using var client = application.CreateClient();

        var email = CreateUniqueEmail("resend-delivery-failure");

        await RegisterAsync(client, email);

        using var response = await client.PostAsJsonAsync(
            "/api/Auth/resend-email-confirmation",
            new
            {
                email
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "If an unverified account exists",
            responseContent);
        Assert.DoesNotContain(
            FailOnSecondEmailVerificationSender.SensitiveDetail,
            responseContent);
        Assert.DoesNotContain(sender.LastToken, responseContent);
    }

    // Updated: Login is blocked before confirmation and succeeds afterward.
    [Fact]
    public async Task Login_WhenEmailIsUnconfirmed_ReturnsUnauthorized()
    {
        var sender = new RecordingEmailVerificationSender();

        using var application = CreateApplication(sender);
        using var client = application.CreateClient();

        var email = CreateUniqueEmail("login-unconfirmed");

        await RegisterAsync(client, email);

        using var response = await client.PostAsJsonAsync(
            "/api/Auth/login",
            new
            {
                email,
                password = ValidPassword
            });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "Please confirm your email before logging in.",
            responseContent);
    }

    [Fact]
    public async Task Login_AfterEmailConfirmation_ReturnsJwtToken()
    {
        var sender = new RecordingEmailVerificationSender();

        using var application = CreateApplication(sender);
        using var client = application.CreateClient();

        var email = CreateUniqueEmail("login-confirmed");

        await RegisterAsync(client, email);

        var verification = Assert.Single(
            sender.Messages,
            message => message.Email == email);

        using var confirmationResponse = await client.GetAsync(
            CreateConfirmationPath(verification));

        Assert.Equal(
            HttpStatusCode.OK,
            confirmationResponse.StatusCode);

        using var loginResponse = await client.PostAsJsonAsync(
            "/api/Auth/login",
            new
            {
                email,
                password = ValidPassword
            });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var authResponse =
            await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        Assert.NotNull(authResponse);
        Assert.False(string.IsNullOrWhiteSpace(authResponse.Token));
        Assert.Equal(3, authResponse.Token.Split('.').Length);
    }

    private WebApplicationFactory<Program> CreateApplication(
        IEmailVerificationSender sender,
        Action<IServiceCollection>? configureServices = null)
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IEmailVerificationSender>();

                services.AddSingleton<IEmailVerificationSender>(sender);

                configureServices?.Invoke(services);
            });
        });
    }

    private static Task<HttpResponseMessage> ConfirmEmailAndLoginAsync(
        HttpClient client,
        VerificationMessage verification)
    {
        return client.PostAsJsonAsync(
            "/api/Auth/confirm-email",
            new
            {
                userId = verification.UserId,
                token = verification.Token
            });
    }

    private static async Task AssertSafeConfirmationFailureAsync(
        HttpResponseMessage response,
        VerificationMessage? verification = null)
    {
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "Invalid or expired verification link.",
            responseContent);
        Assert.DoesNotContain("eyJ", responseContent);

        if (verification != null)
        {
            Assert.DoesNotContain(verification.UserId, responseContent);
            Assert.DoesNotContain(verification.Token, responseContent);
        }
    }

    private static async Task RegisterAsync(
        HttpClient client,
        string email)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                fullName = "Email Verification Test",
                email,
                password = ValidPassword
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static async Task<ApplicationUser> GetUserAsync(
        WebApplicationFactory<Program> application,
        string email)
    {
        using var scope = application.Services.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByEmailAsync(email);

        return Assert.IsType<ApplicationUser>(user);
    }

    private static string CreateConfirmationPath(
        VerificationMessage verification)
    {
        return "/api/Auth/confirm-email" +
               $"?userId={Uri.EscapeDataString(verification.UserId)}" +
               $"&token={Uri.EscapeDataString(verification.Token)}";
    }

    private static string CreateUniqueEmail(string prefix)
    {
        return $"{prefix}-{Guid.NewGuid():N}@example.com";
    }

    private sealed class RecordingEmailVerificationSender :
        IEmailVerificationSender
    {
        private readonly List<VerificationMessage> _messages = [];

        public IReadOnlyList<VerificationMessage> Messages => _messages;

        public Task SendVerificationLinkAsync(
            ApplicationUser user,
            string token,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _messages.Add(new VerificationMessage(
                user.Email ?? string.Empty,
                user.Id,
                token));

            return Task.CompletedTask;
        }
    }

    private sealed record VerificationMessage(
        string Email,
        string UserId,
        string Token);

    private sealed class FailOnFirstEmailVerificationSender :
        IEmailVerificationSender
    {
        public const string SensitiveDetail =
            "test-only-sensitive-provider-detail";

        public string ConfirmationToken { get; private set; } = string.Empty;

        public int SendCount { get; private set; }

        public Task SendVerificationLinkAsync(
            ApplicationUser user,
            string token,
            CancellationToken cancellationToken = default)
        {
            SendCount++;
            ConfirmationToken = token;

            if (SendCount == 1)
            {
                throw new EmailDeliveryException(SensitiveDetail);
            }

            return Task.CompletedTask;
        }
    }

    private sealed class FailOnSecondEmailVerificationSender :
        IEmailVerificationSender
    {
        public const string SensitiveDetail =
            "test-only-sensitive-provider-detail";

        private int _sendCount;

        public string LastToken { get; private set; } = string.Empty;

        public Task SendVerificationLinkAsync(
            ApplicationUser user,
            string token,
            CancellationToken cancellationToken = default)
        {
            _sendCount++;
            LastToken = token;

            if (_sendCount > 1)
            {
                throw new EmailDeliveryException(SensitiveDetail);
            }

            return Task.CompletedTask;
        }
    }
}
