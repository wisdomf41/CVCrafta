using ResumeApp.Server.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net.Http.Headers;

namespace ResumeApp.Server.Tests.Integration;

public class ResumeApiIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ResumeApiIntegrationTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // First HTTP integration test for the Resume API.
    [Fact]
    public async Task GetAll_WhenRequested_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/Resume");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenResumeDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/Resume/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/Resume",
            new { });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // Verifies that unauthenticated users receive 401 when accessing their personal resume.
    [Fact]
    public async Task GetMy_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/Resume/my");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // Verifies that a user with valid registration details receives 200 OK.
    [Fact]
    public async Task Register_WhenDetailsAreValid_ReturnsOk()
    {
        var registration = new
        {
            FullName = "Integration Test User",
            Email = $"integration-{Guid.NewGuid()}@example.com",
            Password = "TestPassword123!"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            registration);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // Verifies that registering the same email twice returns 400 Bad Request.
    [Fact]
    public async Task Register_WhenEmailAlreadyExists_ReturnsBadRequest()
    {
        var registration = new
        {
            FullName = "Duplicate Test User",
            Email = $"duplicate-{Guid.NewGuid()}@example.com",
            Password = "TestPassword123!"
        };

        await _client.PostAsJsonAsync("/api/Auth/register", registration);

        var secondResponse = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            registration);

        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    // Verifies that a registered user can log in with valid credentials and receive 200 OK.
    [Fact]
    public async Task Login_WhenCredentialsAreValid_ReturnsOk()
    {
        var email = $"login-{Guid.NewGuid()}@example.com";
        const string password = "TestPassword123!";

        var registration = new
        {
            FullName = "Login Test User",
            Email = email,
            Password = password
        };

        var login = new
        {
            Email = email,
            Password = password
        };

        var registrationResponse = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            registration);

        Assert.Equal(HttpStatusCode.OK, registrationResponse.StatusCode);

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/Auth/login",
            login);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
    }

    // Verifies that invalid login credentials return 401 Unauthorized.
    [Fact]
    public async Task Login_WhenCredentialsAreInvalid_ReturnsUnauthorized()
    {
        var login = new
        {
            Email = $"unknown-{Guid.NewGuid()}@example.com",
            Password = "WrongPassword123!"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/Auth/login",
            login);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // Verifies that unauthenticated users cannot update a resume.
    [Fact]
    public async Task Update_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var response = await _client.PutAsJsonAsync(
            "/api/Resume/1",
            new { });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // Verifies that unauthenticated users cannot delete a resume.
    [Fact]
    public async Task Delete_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        var response = await _client.DeleteAsync("/api/Resume/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // Verifies that successful login returns a valid, non-empty JWT.
    [Fact]
    public async Task Login_WhenCredentialsAreValid_ReturnsJwtToken()
    {
        var email = $"jwt-{Guid.NewGuid()}@example.com";
        const string password = "TestPassword123!";

        var registration = new
        {
            FullName = "JWT Test User",
            Email = email,
            Password = password
        };

        var login = new
        {
            Email = email,
            Password = password
        };

        var registrationResponse = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            registration);

        Assert.Equal(HttpStatusCode.OK, registrationResponse.StatusCode);

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/Auth/login",
            login);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var responseContent = await loginResponse.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(responseContent);

        var token = document.RootElement.GetProperty("token").GetString();

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Equal(3, token.Split('.').Length);
    }

    // Verifies that an authenticated user can retrieve their personal resume.
    [Fact]
    public async Task GetMy_WhenUserIsAuthenticated_ReturnsOk()
    {
        var token = await RegisterAndLoginAsync("my-resumes");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "My Resume Test User",
                Title = "Full-Stack .NET Developer",
                Email = "my-resume@example.com",
                Url = "https://example.com/my-resume",
                Stack = ".NET, React, PostgreSQL",
                Country = "Nigeria",
                Summary = "Resume used to test the personal resume endpoint."
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var response = await _client.GetAsync("/api/Resume/my");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // Verifies that an authenticated user can create a resume.
    [Fact]
    public async Task Create_WhenUserIsAuthenticated_ReturnsCreated()
    {
        var token = await RegisterAndLoginAsync("create-resume");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var resume = new
        {
            Name = "Integration Test User",
            Title = "Full-Stack .NET Developer",
            Email = "integration-resume@example.com",
            Url = "https://example.com/integration-resume",
            Stack = ".NET, React, PostgreSQL",
            Country = "Nigeria",
            Summary = "Resume created by an authenticated integration test."
        };

        var response = await _client.PostAsJsonAsync(
            "/api/Resume",
            resume);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    private async Task<string> RegisterAndLoginAsync(string emailPrefix)
    {
        var email = $"{emailPrefix}-{Guid.NewGuid()}@example.com";
        const string password = "TestPassword123!";

        var registrationResponse = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                FullName = "Authenticated Test User",
                Email = email,
                Password = password
            });

        Assert.Equal(HttpStatusCode.OK, registrationResponse.StatusCode);

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/Auth/login",
            new
            {
                Email = email,
                Password = password
            });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var responseContent = await loginResponse.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(responseContent);

        var token = document.RootElement
            .GetProperty("token")
            .GetString();

        Assert.False(string.IsNullOrWhiteSpace(token));

        return token!;
    }

    // Verifies that a newly created resume can be retrieved by its ID.
    [Fact]
    public async Task GetById_WhenResumeExists_ReturnsOk()
    {
        var token = await RegisterAndLoginAsync("get-resume");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Retrieve Test User",
                Title = ".NET Developer",
                Email = "retrieve-test@example.com",
                Url = "https://example.com/retrieve-test",
                Stack = ".NET, React, PostgreSQL",
                Country = "Nigeria",
                Summary = "Resume created for the retrieval test."
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        var response = await _client.GetAsync($"/api/Resume/{resumeId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // Verifies that an authenticated owner can update their resume.
    [Fact]
    public async Task Update_WhenUserOwnsResume_ReturnsNoContent()
    {
        var token = await RegisterAndLoginAsync("update-resume");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Update Test User",
                Title = "Junior .NET Developer",
                Email = "update-test@example.com",
                Url = "https://example.com/update-test",
                Stack = ".NET, React",
                Country = "Nigeria",
                Summary = "Resume before the update."
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/Resume/{resumeId}",
            new
            {
                Name = "Update Test User",
                Title = "Full-Stack .NET Developer",
                Email = "update-test@example.com",
                Url = "https://example.com/update-test",
                Stack = ".NET, React, PostgreSQL",
                Country = "Nigeria",
                Summary = "Resume successfully updated."
            });

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/Resume/{resumeId}");
        var updatedContent = await getResponse.Content.ReadAsStringAsync();

        using var updatedDocument = JsonDocument.Parse(updatedContent);

        Assert.Equal(
            "Full-Stack .NET Developer",
            updatedDocument.RootElement.GetProperty("title").GetString());
    }

    // Verifies that an authenticated owner can delete their resume.
    [Fact]
    public async Task Delete_WhenUserOwnsResume_ReturnsNoContent()
    {
        var token = await RegisterAndLoginAsync("delete-resume");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Delete Test User",
                Title = ".NET Developer",
                Email = "delete-test@example.com",
                Url = "https://example.com/delete-test",
                Stack = ".NET, React, PostgreSQL",
                Country = "Nigeria",
                Summary = "Resume created for the deletion test."
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        var deleteResponse =
            await _client.DeleteAsync($"/api/Resume/{resumeId}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    // Verifies that a deleted resume can no longer be retrieved.
    [Fact]
    public async Task GetById_WhenResumeHasBeenDeleted_ReturnsNotFound()
    {
        var token = await RegisterAndLoginAsync("deleted-resume");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Deleted Resume Test User",
                Title = ".NET Developer",
                Email = "deleted-resume@example.com",
                Url = "https://example.com/deleted-resume",
                Stack = ".NET, React, PostgreSQL",
                Country = "Nigeria",
                Summary = "Resume created and deleted during testing."
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        var deleteResponse =
            await _client.DeleteAsync($"/api/Resume/{resumeId}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/Resume/{resumeId}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    // Verifies that one user cannot update a resume belonging to another user.
    [Fact]
    public async Task Update_WhenResumeBelongsToAnotherUser_ReturnsNotFound()
    {
        var ownerToken = await RegisterAndLoginAsync("update-owner");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", ownerToken);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Resume Owner",
                Title = ".NET Developer",
                Email = "update-owner@example.com",
                Url = "https://example.com/update-owner",
                Stack = ".NET, React",
                Country = "Nigeria",
                Summary = "Resume belonging to the first user."
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        var otherUserToken = await RegisterAndLoginAsync("update-other-user");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", otherUserToken);

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/Resume/{resumeId}",
            new
            {
                Name = "Unauthorized User",
                Title = "Changed Title",
                Email = "other-user@example.com",
                Url = "https://example.com/other-user",
                Stack = ".NET",
                Country = "Nigeria",
                Summary = "This update must be rejected."
            });

        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
    }

    // Verifies that one user cannot delete a resume belonging to another user.
    [Fact]
    public async Task Delete_WhenResumeBelongsToAnotherUser_ReturnsNotFound()
    {
        var ownerToken = await RegisterAndLoginAsync("delete-owner");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", ownerToken);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Delete Resume Owner",
                Title = ".NET Developer",
                Email = "delete-owner@example.com",
                Url = "https://example.com/delete-owner",
                Stack = ".NET, React",
                Country = "Nigeria",
                Summary = "Resume protected from deletion by another user."
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        var otherUserToken = await RegisterAndLoginAsync("delete-other-user");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", otherUserToken);

        var deleteResponse =
            await _client.DeleteAsync($"/api/Resume/{resumeId}");

        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    // Verifies that updating a nonexistent resume returns 404 Not Found.
    [Fact]
    public async Task Update_WhenResumeDoesNotExist_ReturnsNotFound()
    {
        var token = await RegisterAndLoginAsync("update-missing");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PutAsJsonAsync(
            "/api/Resume/999999",
            new
            {
                Name = "Missing Resume User",
                Title = ".NET Developer",
                Email = "missing-update@example.com",
                Url = "https://example.com/missing-update",
                Stack = ".NET, React",
                Country = "Nigeria",
                Summary = "This resume does not exist."
            });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // Verifies that deleting a nonexistent resume returns 404 Not Found.
    [Fact]
    public async Task Delete_WhenResumeDoesNotExist_ReturnsNotFound()
    {
        var token = await RegisterAndLoginAsync("delete-missing");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.DeleteAsync("/api/Resume/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // Verifies that registration without the required email returns 400 Bad Request.
    [Fact]
    public async Task Register_WhenEmailIsMissing_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                FullName = "Missing Email User",
                Password = "TestPassword123!"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Verifies that registration without the required password returns 400 Bad Request.
    [Fact]
    public async Task Register_WhenPasswordIsMissing_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                FullName = "Missing Password User",
                Email = $"missing-password-{Guid.NewGuid()}@example.com"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    // Verifies that an existing user cannot log in with an incorrect password.
    [Fact]
    public async Task Login_WhenPasswordIsIncorrect_ReturnsUnauthorized()
    {
        var email = $"wrong-password-{Guid.NewGuid()}@example.com";

        var registrationResponse = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                FullName = "Wrong Password Test User",
                Email = email,
                Password = "TestPassword123!"
            });

        Assert.Equal(HttpStatusCode.OK, registrationResponse.StatusCode);

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/Auth/login",
            new
            {
                Email = email,
                Password = "IncorrectPassword123!"
            });

        Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
    }

    // Verifies that registration rejects passwords exceeding the 50-character limit.
    [Fact]
    public async Task Register_WhenPasswordExceedsMaximumLength_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                FullName = "Long Password Test User",
                Email = $"long-password-{Guid.NewGuid()}@example.com",
                Password = "TestPassword123!" + new string('A', 35)
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Verifies that an authenticated user without a resume receives 404 Not Found.
    [Fact]
    public async Task GetMy_WhenAuthenticatedUserHasNoResume_ReturnsNotFound()
    {
        var token = await RegisterAndLoginAsync("no-resume");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/Resume/my");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // Verifies that creating a resume returns the saved resume details.
    [Fact]
    public async Task Create_WhenDetailsAreValid_ReturnsCreatedResumeData()
    {
        var token = await RegisterAndLoginAsync("create-data");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Created Resume User",
                Title = "Backend .NET Developer",
                Email = "created-data@example.com",
                Url = "https://example.com/created-data",
                Stack = ".NET, PostgreSQL, Docker",
                Country = "Nigeria",
                Summary = "Testing the returned resume data."
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);

        Assert.True(document.RootElement.GetProperty("id").GetInt32() > 0);
        Assert.Equal(
            "Backend .NET Developer",
            document.RootElement.GetProperty("title").GetString());
        Assert.Equal(
            ".NET, PostgreSQL, Docker",
            document.RootElement.GetProperty("stack").GetString());
    }

    // Verifies that each authenticated user receives only the resume belonging to their account.
    [Fact]
    public async Task GetMy_WhenMultipleUsersHaveResumes_ReturnsOnlyCurrentUsersResume()
    {
        var firstToken = await RegisterAndLoginAsync("first-resume-user");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", firstToken);

        var firstCreateResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "First User Resume",
                Title = ".NET Developer",
                Email = "first-user@example.com",
                Url = "https://example.com/first-user",
                Stack = ".NET",
                Country = "Nigeria",
                Summary = "Belongs to the first user."
            });

        Assert.Equal(HttpStatusCode.Created, firstCreateResponse.StatusCode);

        var secondToken = await RegisterAndLoginAsync("second-resume-user");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", secondToken);

        var secondCreateResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Second User Resume",
                Title = "Full-Stack Developer",
                Email = "second-user@example.com",
                Url = "https://example.com/second-user",
                Stack = ".NET, React",
                Country = "Nigeria",
                Summary = "Belongs to the second user."
            });

        Assert.Equal(HttpStatusCode.Created, secondCreateResponse.StatusCode);

        var response = await _client.GetAsync("/api/Resume/my");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);

        Assert.Equal(JsonValueKind.Object, document.RootElement.ValueKind);
        Assert.Equal(
            "Second User Resume",
            document.RootElement.GetProperty("name").GetString());
        Assert.NotEqual(
            "First User Resume",
            document.RootElement.GetProperty("name").GetString());
    }

    // Verifies that a rejected cross-user update leaves the original resume unchanged.
    [Fact]
    public async Task Update_WhenResumeBelongsToAnotherUser_DoesNotChangeResume()
    {
        var ownerToken = await RegisterAndLoginAsync("unchanged-owner");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", ownerToken);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Protected Resume Owner",
                Title = "Original Title",
                Email = "protected-update@example.com",
                Url = "https://example.com/protected-update",
                Stack = ".NET",
                Country = "Nigeria",
                Summary = "The original resume."
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        var otherUserToken = await RegisterAndLoginAsync("unchanged-other");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", otherUserToken);

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/Resume/{resumeId}",
            new
            {
                Name = "Unauthorized User",
                Title = "Unauthorized Changed Title",
                Email = "unauthorized-update@example.com",
                Url = "https://example.com/unauthorized-update",
                Stack = "Changed Stack",
                Country = "Nigeria",
                Summary = "This change must not be saved."
            });

        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/Resume/{resumeId}");
        var getContent = await getResponse.Content.ReadAsStringAsync();

        using var getDocument = JsonDocument.Parse(getContent);

        Assert.Equal(
            "Original Title",
            getDocument.RootElement.GetProperty("title").GetString());
    }

    // Verifies that a rejected cross-user deletion leaves the resume available.
    [Fact]
    public async Task Delete_WhenResumeBelongsToAnotherUser_DoesNotDeleteResume()
    {
        var ownerToken = await RegisterAndLoginAsync("preserved-owner");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", ownerToken);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Preserved Resume Owner",
                Title = ".NET Developer",
                Email = "preserved-delete@example.com",
                Url = "https://example.com/preserved-delete",
                Stack = ".NET, React",
                Country = "Nigeria",
                Summary = "This resume must remain available."
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        var otherUserToken = await RegisterAndLoginAsync("preserved-other");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", otherUserToken);

        var deleteResponse =
            await _client.DeleteAsync($"/api/Resume/{resumeId}");

        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/Resume/{resumeId}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    // Verifies that anyone can access the public resume list.
    [Fact]
    public async Task GetAll_WithoutAuthentication_ReturnsOk()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync("/api/Resume");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // Verifies that an unauthenticated user cannot access personal resumes.
    [Fact]
    public async Task GetMy_WithoutAuthentication_ReturnsUnauthorized()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync("/api/Resume/my");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // Verifies that an unauthenticated user cannot create a resume.
    [Fact]
    public async Task Create_WithoutAuthentication_ReturnsUnauthorized()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Unauthenticated User",
                Title = ".NET Developer",
                Email = "unauthenticated@example.com",
                Url = "https://example.com/unauthenticated",
                Stack = ".NET, React",
                Country = "Nigeria",
                Summary = "This resume must not be created."
            });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // Verifies that an existing resume can be viewed publicly without authentication.
    [Fact]
    public async Task GetById_WhenResumeExists_AllowsPublicAccess()
    {
        var token = await RegisterAndLoginAsync("public-resume");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Public Resume User",
                Title = "Full-Stack .NET Developer",
                Email = "public-resume@example.com",
                Url = "https://example.com/public-resume",
                Stack = ".NET, React, PostgreSQL",
                Country = "Nigeria",
                Summary = "A publicly accessible resume."
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync($"/api/Resume/{resumeId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // Verifies that the public resume-list response is a JSON array.
    [Fact]
    public async Task GetAll_WhenRequested_ReturnsJsonArray()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync("/api/Resume");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);

        Assert.Equal(JsonValueKind.Array, document.RootElement.ValueKind);
    }

    // Verifies that retrieving a resume returns its stored field values.
    [Fact]
    public async Task GetById_WhenResumeExists_ReturnsPersistedData()
    {
        var token = await RegisterAndLoginAsync("persisted-resume");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Persisted Resume User",
                Title = "Backend Developer",
                Email = "persisted@example.com",
                Url = "https://example.com/persisted",
                Stack = ".NET, SQL Server, Redis",
                Country = "Nigeria",
                Summary = "Testing persisted resume data."
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        _client.DefaultRequestHeaders.Authorization = null;

        var getResponse = await _client.GetAsync($"/api/Resume/{resumeId}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var getContent = await getResponse.Content.ReadAsStringAsync();
        using var getDocument = JsonDocument.Parse(getContent);

        var resume = getDocument.RootElement;

        Assert.Equal(
            "Persisted Resume User",
            resume.GetProperty("name").GetString());

        Assert.Equal(
            "Backend Developer",
            resume.GetProperty("title").GetString());

        Assert.Equal(
            "persisted@example.com",
            resume.GetProperty("email").GetString());

        Assert.Equal(
            "https://example.com/persisted",
            resume.GetProperty("url").GetString());

        Assert.Equal(
            ".NET, SQL Server, Redis",
            resume.GetProperty("stack").GetString());

        Assert.Equal(
            "Nigeria",
            resume.GetProperty("country").GetString());

        Assert.Equal(
            "Testing persisted resume data.",
            resume.GetProperty("summary").GetString());
    }

    // Verifies that nullable resume fields may be omitted during creation.
    [Fact]
    public async Task Create_WhenOptionalFieldsAreOmitted_ReturnsCreated()
    {
        var token = await RegisterAndLoginAsync("optional-create");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Optional Fields User",
                Title = ".NET Developer",
                Url = "https://example.com/optional-fields",
                Stack = ".NET, React"
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);

        Assert.True(document.RootElement.GetProperty("id").GetInt32() > 0);
        Assert.Equal(
            "Optional Fields User",
            document.RootElement.GetProperty("name").GetString());
    }

    // Verifies that an owner can clear nullable resume fields during an update.
    [Fact]
    public async Task Update_WhenOptionalFieldsAreNull_ClearsOptionalFields()
    {
        var token = await RegisterAndLoginAsync("clear-optional");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Clear Optional Fields User",
                Title = ".NET Developer",
                Email = "clear-fields@example.com",
                Url = "https://example.com/clear-fields",
                Stack = ".NET, React",
                Country = "Nigeria",
                Summary = "These optional fields will be cleared."
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/Resume/{resumeId}",
            new
            {
                Name = "Clear Optional Fields User",
                Title = "Full-Stack .NET Developer",
                Email = (string?)null,
                Url = "https://example.com/clear-fields",
                Stack = ".NET, React, SQL Server",
                Country = (string?)null,
                Summary = (string?)null
            });

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/Resume/{resumeId}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var getContent = await getResponse.Content.ReadAsStringAsync();
        using var getDocument = JsonDocument.Parse(getContent);

        var resume = getDocument.RootElement;

        Assert.True(
            !resume.TryGetProperty("email", out var email) ||
            email.ValueKind == JsonValueKind.Null);

        Assert.True(
            !resume.TryGetProperty("country", out var country) ||
            country.ValueKind == JsonValueKind.Null);

        Assert.True(
            !resume.TryGetProperty("summary", out var summary) ||
            summary.ValueKind == JsonValueKind.Null);
    }

    // Verifies that a malformed JWT cannot access a protected endpoint.
    [Fact]
    public async Task GetMy_WhenBearerTokenIsMalformed_ReturnsUnauthorized()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                "this-is-not-a-valid-jwt");

        var response = await _client.GetAsync("/api/Resume/my");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // Verifies that registration requires the user's full name.
    [Fact]
    public async Task Register_WhenFullNameIsMissing_ReturnsBadRequest()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                Email = $"missing-name-{Guid.NewGuid()}@example.com",
                Password = "TestPassword123!"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Verifies that registration rejects an invalid email address.
    [Fact]
    public async Task Register_WhenEmailFormatIsInvalid_ReturnsBadRequest()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                FullName = "Invalid Email User",
                Email = "not-a-valid-email",
                Password = "TestPassword123!"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Verifies that registration rejects a password below the minimum length.
    [Fact]
    public async Task Register_WhenPasswordIsTooShort_ReturnsBadRequest()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                FullName = "Short Password User",
                Email = $"short-password-{Guid.NewGuid()}@example.com",
                Password = "Ab1!"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Verifies that login requires an email address.
    [Fact]
    public async Task Login_WhenEmailIsMissing_ReturnsBadRequest()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.PostAsJsonAsync(
            "/api/Auth/login",
            new
            {
                Password = "TestPassword123!"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Verifies that login requires a password.
    [Fact]
    public async Task Login_WhenPasswordIsMissing_ReturnsBadRequest()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.PostAsJsonAsync(
            "/api/Auth/login",
            new
            {
                Email = $"missing-login-password-{Guid.NewGuid()}@example.com"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Verifies that a resume cannot be created without a name.
    [Fact]
    public async Task Create_WhenNameIsMissing_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync("missing-resume-name");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Title = ".NET Developer",
                Email = "missing-name@example.com",
                Url = "https://example.com/missing-name",
                Stack = ".NET, React",
                Country = "Nigeria",
                Summary = "The required name is missing."
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Verifies that a resume cannot be created without a title.
    [Fact]
    public async Task Create_WhenTitleIsMissing_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync("missing-resume-title");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Missing Title User",
                Email = "missing-title@example.com",
                Url = "https://example.com/missing-title",
                Stack = ".NET, React",
                Country = "Nigeria",
                Summary = "The required title is missing."
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Verifies that a resume cannot be created without a URL.
    [Fact]
    public async Task Create_WhenUrlIsMissing_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync("missing-resume-url");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Missing URL User",
                Title = ".NET Developer",
                Email = "missing-url@example.com",
                Stack = ".NET, React",
                Country = "Nigeria",
                Summary = "The required URL is missing."
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Verifies that a resume cannot be created without a technology stack.
    [Fact]
    public async Task Create_WhenStackIsMissing_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync("missing-resume-stack");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Missing Stack User",
                Title = ".NET Developer",
                Email = "missing-stack@example.com",
                Url = "https://example.com/missing-stack",
                Country = "Nigeria",
                Summary = "The required technology stack is missing."
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Verifies that an existing resume cannot be updated without a name.
    [Fact]
    public async Task Update_WhenNameIsMissing_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync("update-missing-name");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Update Validation User",
                Title = ".NET Developer",
                Email = "update-missing-name@example.com",
                Url = "https://example.com/update-missing-name",
                Stack = ".NET, React",
                Country = "Nigeria",
                Summary = "Resume created before testing an invalid update."
            });

        var validationResponse =
        await createResponse.Content.ReadAsStringAsync();

        Assert.True(
            createResponse.StatusCode == HttpStatusCode.Created,
            $"Expected Created but received {createResponse.StatusCode}. Response: {validationResponse}"); // CORRECTED: Shows which field failed validation.

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/Resume/{resumeId}",
            new
            {
                Title = "Full-Stack .NET Developer",
                Email = "update-missing-name@example.com",
                Url = "https://example.com/update-missing-name",
                Stack = ".NET, React, PostgreSQL",
                Country = "Nigeria",
                Summary = "This update must fail because the name is missing."
            });

        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }

    // Covers whitespace values and missing required fields during resume updates.
    [Fact]
    public async Task Create_WhenNameIsWhitespace_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync("whitespace-name");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "   ",
                Title = ".NET Developer",
                Url = "https://example.com/whitespace-name",
                Stack = ".NET, React"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WhenStackIsWhitespace_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync("whitespace-stack");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Whitespace Stack User",
                Title = ".NET Developer",
                Url = "https://example.com/whitespace-stack",
                Stack = "   "
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenTitleIsMissing_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync("update-missing-title");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Update Title User",
                Title = ".NET Developer",
                Url = "https://example.com/update-title",
                Stack = ".NET, React"
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/Resume/{resumeId}",
            new
            {
                Name = "Update Title User",
                Url = "https://example.com/update-title",
                Stack = ".NET, React, SQL Server"
            });

        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }

    [Fact]
    public async Task Update_WhenUrlIsMissing_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync("update-missing-url");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Update URL User",
                Title = ".NET Developer",
                Url = "https://example.com/update-url",
                Stack = ".NET, React"
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/Resume/{resumeId}",
            new
            {
                Name = "Update URL User",
                Title = "Full-Stack .NET Developer",
                Stack = ".NET, React, SQL Server"
            });

        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }

    [Fact]
    public async Task Update_WhenStackIsMissing_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync("update-missing-stack");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Update Stack User",
                Title = ".NET Developer",
                Url = "https://example.com/update-stack",
                Stack = ".NET, React"
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/Resume/{resumeId}",
            new
            {
                Name = "Update Stack User",
                Title = "Full-Stack .NET Developer",
                Url = "https://example.com/update-stack"
            });

        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }

    // Verifies that a resume cannot be created when the title contains only whitespace.
    [Fact]
    public async Task Create_WhenTitleIsWhitespace_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync("create-whitespace-title");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Whitespace Title User",
                Title = "   ",
                Url = "https://example.com/whitespace-title",
                Stack = ".NET, React"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Verifies that a resume cannot be created when the URL contains only whitespace.
    [Fact]
    public async Task Create_WhenUrlIsWhitespace_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync("create-whitespace-url");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Whitespace URL User",
                Title = ".NET Developer",
                Url = "   ",
                Stack = ".NET, React"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Verifies that a resume cannot be updated when the name contains only whitespace.
    [Fact]
    public async Task Update_WhenNameIsWhitespace_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync("update-whitespace-name");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Original Name User",
                Title = ".NET Developer",
                Url = "https://example.com/update-whitespace-name",
                Stack = ".NET, React"
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/Resume/{resumeId}",
            new
            {
                Name = "   ",
                Title = "Full-Stack .NET Developer",
                Url = "https://example.com/update-whitespace-name",
                Stack = ".NET, React, SQL Server"
            });

        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }

    // Verifies that a resume cannot be updated when the title contains only whitespace.
    [Fact]
    public async Task Update_WhenTitleIsWhitespace_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync("update-whitespace-title");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Whitespace Update Title User",
                Title = ".NET Developer",
                Url = "https://example.com/update-whitespace-title",
                Stack = ".NET, React"
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/Resume/{resumeId}",
            new
            {
                Name = "Whitespace Update Title User",
                Title = "   ",
                Url = "https://example.com/update-whitespace-title",
                Stack = ".NET, React, SQL Server"
            });

        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }

    // Verifies that a resume cannot be updated when the URL contains only whitespace.
    [Fact]
    public async Task Update_WhenUrlIsWhitespace_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync("update-whitespace-url");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Resume",
            new
            {
                Name = "Whitespace Update URL User",
                Title = ".NET Developer",
                Url = "https://example.com/update-whitespace-url",
                Stack = ".NET, React"
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        using var createDocument = JsonDocument.Parse(createContent);

        var resumeId = createDocument.RootElement
            .GetProperty("id")
            .GetInt32();

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/Resume/{resumeId}",
            new
            {
                Name = "Whitespace Update URL User",
                Title = "Full-Stack .NET Developer",
                Url = "   ",
                Stack = ".NET, React, SQL Server"
            });

        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }
}
