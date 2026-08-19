using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ResumeApp.Server.Data;

namespace ResumeApp.Server.Tests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string JwtEnvironmentVariable = "Jwt__Key";

    private const string TestJwtKey =
        "ResumeAppIntegrationTestsOnlySigningKey2026AtLeast32Characters";

    private readonly string _databaseName =
        $"ResumeAppIntegrationTests_{Guid.NewGuid()}";

    private readonly string? _originalJwtKey;

    public CustomWebApplicationFactory()
    {
        // Provide the test JWT key before Program.cs starts.
        _originalJwtKey =
            Environment.GetEnvironmentVariable(JwtEnvironmentVariable);

        Environment.SetEnvironmentVariable(
            JwtEnvironmentVariable,
            TestJwtKey);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ResumeDbContext>>();
            services.RemoveAll<
                IDbContextOptionsConfiguration<ResumeDbContext>>();

            services.AddDbContext<ResumeDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            services.RemoveAll<IDistributedCache>();
            services.AddDistributedMemoryCache();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        Environment.SetEnvironmentVariable(
            JwtEnvironmentVariable,
            _originalJwtKey);
    }
}
