using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ResumeApp.Server.Data;
using ResumeApp.Server.DTOs.Resume;
using ResumeApp.Server.Model;
using ResumeApp.Server.Services.Implementations;
using System.Text.Json;

namespace ResumeApp.Server.Tests;

public class ResumeServiceTests
{
    [Fact]
    public async Task UpdateAsync_WhenResumeBelongsToAnotherUser_ReturnsFalse()
    {
        var databaseOptions = new DbContextOptionsBuilder<ResumeDbContext>()
            .UseInMemoryDatabase($"ResumeAppTests_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ResumeDbContext(databaseOptions);

        dbContext.Resumes.Add(new Resume
        {
            Id = 1,
            Name = "Test User",
            Title = ".NET Developer",
            Email = "test@example.com",
            Url = "https://example.com",
            Stack = "C#, ASP.NET Core",
            Country = "Nigeria",
            Summary = "Test resume",
            UserId = "owner-user-id",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();

        IDistributedCache cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));

        var service = new ResumeService(dbContext, cache);

        var updateDto = new UpdateResumeDto
        {
            Name = "Unauthorized Update",
            Title = "Updated Title",
            Email = "updated@example.com",
            Url = "https://updated.example.com",
            Stack = "React, .NET",
            Country = "Nigeria",
            Summary = "This update should not be allowed"
        };

        var result = await service.UpdateAsync(
            id: 1,
            dto: updateDto,
            userId: "different-user-id");

        Assert.False(result);

        var unchangedResume = await dbContext.Resumes.FindAsync(1);

        Assert.NotNull(unchangedResume);
        Assert.Equal("Test User", unchangedResume.Name);
        Assert.Equal(".NET Developer", unchangedResume.Title);
    }

    // Tests a successful owner update and cache invalidation.
    [Fact]
    public async Task UpdateAsync_WhenResumeBelongsToUser_UpdatesResumeAndInvalidatesCache()
    {
        var databaseOptions = new DbContextOptionsBuilder<ResumeDbContext>()
            .UseInMemoryDatabase($"ResumeAppTests_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ResumeDbContext(databaseOptions);

        dbContext.Resumes.Add(new Resume
        {
            Id = 1,
            Name = "Original Name",
            Title = ".NET Developer",
            Email = "original@example.com",
            Url = "https://example.com",
            Stack = "C#, ASP.NET Core",
            Country = "Nigeria",
            Summary = "Original summary",
            UserId = "owner-user-id",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();

        IDistributedCache cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));

        await cache.SetStringAsync(
            "resumes:all",
            "[{\"id\":1,\"name\":\"Original Name\"}]");

        await cache.SetStringAsync(
            "resume:1",
            "{\"id\":1,\"name\":\"Original Name\"}");

        var service = new ResumeService(dbContext, cache);

        var updateDto = new UpdateResumeDto
        {
            Name = "Updated Name",
            Title = "Backend .NET Developer",
            Email = "updated@example.com",
            Url = "https://updated.example.com",
            Stack = "C#, ASP.NET Core, Redis",
            Country = "Nigeria",
            Summary = "Updated summary"
        };

        var result = await service.UpdateAsync(
            id: 1,
            dto: updateDto,
            userId: "owner-user-id");

        Assert.True(result);

        var updatedResume = await dbContext.Resumes.FindAsync(1);

        Assert.NotNull(updatedResume);
        Assert.Equal("Updated Name", updatedResume.Name);
        Assert.Equal("Backend .NET Developer", updatedResume.Title);
        Assert.Equal("C#, ASP.NET Core, Redis", updatedResume.Stack);

        Assert.Null(await cache.GetStringAsync("resumes:all"));
        Assert.Null(await cache.GetStringAsync("resume:1"));
    }

    [Fact]
    public async Task DeleteAsync_WhenResumeBelongsToAnotherUser_ReturnsFalse()
    {
        // Verify another user cannot delete the resume or clear its cache.
        var databaseOptions = new DbContextOptionsBuilder<ResumeDbContext>()
            .UseInMemoryDatabase($"ResumeAppTests_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ResumeDbContext(databaseOptions);

        dbContext.Resumes.Add(new Resume
        {
            Id = 1,
            Name = "Protected Resume",
            Title = ".NET Developer",
            Email = "owner@example.com",
            Url = "https://example.com",
            Stack = "C#, ASP.NET Core",
            Country = "Nigeria",
            Summary = "This resume belongs to another user.",
            UserId = "owner-user-id",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();

        IDistributedCache cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));

        await cache.SetStringAsync(
            "resumes:all",
            "[{\"id\":1,\"name\":\"Protected Resume\"}]");

        await cache.SetStringAsync(
            "resume:1",
            "{\"id\":1,\"name\":\"Protected Resume\"}");

        var service = new ResumeService(dbContext, cache);

        var result = await service.DeleteAsync(
            id: 1,
            userId: "different-user-id");

        Assert.False(result);

        var existingResume = await dbContext.Resumes.FindAsync(1);

        Assert.NotNull(existingResume);
        Assert.Equal("Protected Resume", existingResume.Name);
        Assert.NotNull(await cache.GetStringAsync("resumes:all"));
        Assert.NotNull(await cache.GetStringAsync("resume:1"));
    }

    // Verify the owner can delete the resume and remove its stale cache entries.
    [Fact]
    public async Task DeleteAsync_WhenResumeBelongsToUser_DeletesResumeAndInvalidatesCache()
    {
        var databaseOptions = new DbContextOptionsBuilder<ResumeDbContext>()
            .UseInMemoryDatabase($"ResumeAppTests_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ResumeDbContext(databaseOptions);

        dbContext.Resumes.Add(new Resume
        {
            Id = 1,
            Name = "Resume To Delete",
            Title = ".NET Developer",
            Email = "owner@example.com",
            Url = "https://example.com",
            Stack = "C#, ASP.NET Core",
            Country = "Nigeria",
            Summary = "This resume should be deleted.",
            UserId = "owner-user-id",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();

        IDistributedCache cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));

        await cache.SetStringAsync(
            "resumes:all",
            "[{\"id\":1,\"name\":\"Resume To Delete\"}]");

        await cache.SetStringAsync(
            "resume:1",
            "{\"id\":1,\"name\":\"Resume To Delete\"}");

        var service = new ResumeService(dbContext, cache);

        var result = await service.DeleteAsync(
            id: 1,
            userId: "owner-user-id");

        Assert.True(result);

        var deletedResume = await dbContext.Resumes.FindAsync(1);

        Assert.Null(deletedResume);
        Assert.Null(await cache.GetStringAsync("resumes:all"));
        Assert.Null(await cache.GetStringAsync("resume:1"));
    }

    // Verify creation assigns ownership and removes the outdated resume-list cache.
    [Fact]
    public async Task CreateAsync_WhenSuccessful_AssignsUserIdAndInvalidatesListCache()
    {
        var databaseOptions = new DbContextOptionsBuilder<ResumeDbContext>()
            .UseInMemoryDatabase($"ResumeAppTests_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ResumeDbContext(databaseOptions);

        IDistributedCache cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));

        await cache.SetStringAsync(
            "resumes:all",
            "[]");

        var service = new ResumeService(dbContext, cache);

        var createDto = new CreateResumeDto
        {
            Name = "New User",
            Title = "Backend .NET Developer",
            Email = "newuser@example.com",
            Url = "https://example.com",
            Stack = "C#, ASP.NET Core, Redis",
            Country = "Nigeria",
            Summary = "A newly created resume."
        };

        var result = await service.CreateAsync(
            createDto,
            userId: "authenticated-user-id");

        Assert.NotNull(result);
        Assert.Equal("New User", result.Name);
        Assert.Equal("Backend .NET Developer", result.Title);

        var createdResume = await dbContext.Resumes.SingleAsync();

        Assert.Equal("authenticated-user-id", createdResume.UserId);
        Assert.Equal("New User", createdResume.Name);
        Assert.Equal("C#, ASP.NET Core, Redis", createdResume.Stack);

        Assert.Null(await cache.GetStringAsync("resumes:all"));
    }

    // Verify cached data is returned without requiring a database record.
    [Fact]
    public async Task GetByIdAsync_WhenResumeExistsInCache_ReturnsCachedResume()
    {
        var databaseOptions = new DbContextOptionsBuilder<ResumeDbContext>()
            .UseInMemoryDatabase($"ResumeAppTests_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ResumeDbContext(databaseOptions);

        IDistributedCache cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));

        var cachedResume = new ResumeResponseDto
        {
            Id = 1,
            Name = "Cached User",
            Title = "Cached .NET Developer",
            Email = "cached@example.com",
            Url = "https://example.com",
            Stack = "C#, Redis",
            Country = "Nigeria",
            Summary = "Returned directly from cache.",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await cache.SetStringAsync(
            "resume:1",
            JsonSerializer.Serialize(cachedResume));

        var service = new ResumeService(dbContext, cache);

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Cached User", result.Name);
        Assert.Equal("Cached .NET Developer", result.Title);

        Assert.Empty(await dbContext.Resumes.ToListAsync());
    }

    // Verify a cache miss falls back to the database and stores the result in cache.
    [Fact]
    public async Task GetByIdAsync_WhenResumeIsNotCached_GetsDatabaseResumeAndCachesIt()
    {
        var databaseOptions = new DbContextOptionsBuilder<ResumeDbContext>()
            .UseInMemoryDatabase($"ResumeAppTests_{Guid.NewGuid()}")
            .Options;
        await using var dbContext = new ResumeDbContext(databaseOptions);

        dbContext.Resumes.Add(new Resume
        {
            Id = 1,
            Name = "Database User",
            Title = "Backend Developer",
            Email = "database@example.com",
            Url = "https://example.com",
            Stack = "C#, ASP.NET Core",
            Country = "Nigeria",
            Summary = "Loaded from the database.",
            UserId = "owner-user-id",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();

        IDistributedCache cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));

        var service = new ResumeService(dbContext, cache);

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Database User", result.Name);
        Assert.Equal("Backend Developer", result.Title);

        var cachedJson = await cache.GetStringAsync("resume:1");

        Assert.NotNull(cachedJson);

        var cachedResume =
            JsonSerializer.Deserialize<ResumeResponseDto>(cachedJson);

        Assert.NotNull(cachedResume);
        Assert.Equal(1, cachedResume.Id);
        Assert.Equal("Database User", cachedResume.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenResumeDoesNotExist_ReturnsNull()
    {
        // Verify a missing resume returns null and is not added to cache.
        var databaseOptions = new DbContextOptionsBuilder<ResumeDbContext>()
            .UseInMemoryDatabase($"ResumeAppTests_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ResumeDbContext(databaseOptions);

        IDistributedCache cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));

        var service = new ResumeService(dbContext, cache);

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
        Assert.Null(await cache.GetStringAsync("resume:999"));
    }

    [Fact]
    public async Task GetAllAsync_WhenResumesExistInCache_ReturnsCachedResumes()
    {
        // Verify the cached resume list is returned without database records.
        var databaseOptions = new DbContextOptionsBuilder<ResumeDbContext>()
            .UseInMemoryDatabase($"ResumeAppTests_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ResumeDbContext(databaseOptions);

        IDistributedCache cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));

        var cachedResumes = new List<ResumeResponseDto>
    {
        new()
        {
            Id = 1,
            Name = "Cached User One",
            Title = ".NET Developer",
            Email = "userone@example.com",
            Url = "https://example.com/one",
            Stack = "C#, ASP.NET Core",
            Country = "Nigeria",
            Summary = "First cached resume.",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        new()
        {
            Id = 2,
            Name = "Cached User Two",
            Title = "React Developer",
            Email = "usertwo@example.com",
            Url = "https://example.com/two",
            Stack = "React, JavaScript",
            Country = "Nigeria",
            Summary = "Second cached resume.",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }
    };

        await cache.SetStringAsync(
            "resumes:all",
            JsonSerializer.Serialize(cachedResumes));

        var service = new ResumeService(dbContext, cache);

        var result = (await service.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("Cached User One", result[0].Name);
        Assert.Equal("Cached User Two", result[1].Name);

        Assert.Empty(await dbContext.Resumes.ToListAsync());
    }

    [Fact]
    public async Task GetAllAsync_WhenCacheIsEmpty_GetsDatabaseResumesAndCachesThem()
    {
        // Verify a list-cache miss loads resumes from the database and caches them.
        var databaseOptions = new DbContextOptionsBuilder<ResumeDbContext>()
            .UseInMemoryDatabase($"ResumeAppTests_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ResumeDbContext(databaseOptions);

        dbContext.Resumes.AddRange(
            new Resume
            {
                Id = 1,
                Name = "Database User One",
                Title = ".NET Developer",
                Email = "userone@example.com",
                Url = "https://example.com/one",
                Stack = "C#, ASP.NET Core",
                Country = "Nigeria",
                Summary = "First database resume.",
                UserId = "user-one-id",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Resume
            {
                Id = 2,
                Name = "Database User Two",
                Title = "React Developer",
                Email = "usertwo@example.com",
                Url = "https://example.com/two",
                Stack = "React, JavaScript",
                Country = "Nigeria",
                Summary = "Second database resume.",
                UserId = "user-two-id",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

        await dbContext.SaveChangesAsync();

        IDistributedCache cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));

        var service = new ResumeService(dbContext, cache);

        var result = (await service.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("Database User One", result[0].Name);
        Assert.Equal("Database User Two", result[1].Name);

        var cachedJson = await cache.GetStringAsync("resumes:all");

        Assert.NotNull(cachedJson);

        var cachedResumes =
            JsonSerializer.Deserialize<List<ResumeResponseDto>>(cachedJson);

        Assert.NotNull(cachedResumes);
        Assert.Equal(2, cachedResumes.Count);
        Assert.Equal("Database User One", cachedResumes[0].Name);
        Assert.Equal("Database User Two", cachedResumes[1].Name);
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenUserOwnsResume_ReturnsUsersResume()
    {
        // Verify the service returns only the resume owned by the requested user.
        var databaseOptions = new DbContextOptionsBuilder<ResumeDbContext>()
            .UseInMemoryDatabase($"ResumeAppTests_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ResumeDbContext(databaseOptions);

        dbContext.Resumes.AddRange(
            new Resume
            {
                Id = 1,
                Name = "First User",
                Title = ".NET Developer",
                Email = "first@example.com",
                Url = "https://example.com/first",
                Stack = "C#, ASP.NET Core",
                Country = "Nigeria",
                Summary = "First user's resume.",
                UserId = "first-user-id",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Resume
            {
                Id = 2,
                Name = "Second User",
                Title = "React Developer",
                Email = "second@example.com",
                Url = "https://example.com/second",
                Stack = "React, JavaScript",
                Country = "Nigeria",
                Summary = "Second user's resume.",
                UserId = "second-user-id",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

        await dbContext.SaveChangesAsync();

        IDistributedCache cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));

        var service = new ResumeService(dbContext, cache);

        var result = await service.GetByUserIdAsync("second-user-id");

        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
        Assert.Equal("Second User", result.Name);
        Assert.Equal("React Developer", result.Title);
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenUserHasNoResume_ReturnsNull()
    {
        // Verify a user without a resume receives no result.
        var databaseOptions = new DbContextOptionsBuilder<ResumeDbContext>()
            .UseInMemoryDatabase($"ResumeAppTests_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ResumeDbContext(databaseOptions);

        dbContext.Resumes.Add(new Resume
        {
            Id = 1,
            Name = "Existing User",
            Title = ".NET Developer",
            Email = "existing@example.com",
            Url = "https://example.com",
            Stack = "C#, ASP.NET Core",
            Country = "Nigeria",
            Summary = "An existing user's resume.",
            UserId = "existing-user-id",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();

        IDistributedCache cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));

        var service = new ResumeService(dbContext, cache);

        var result = await service.GetByUserIdAsync("user-without-resume");

        Assert.Null(result);
    }

    // NEW: Additional ResumeService edge-case and cache-behaviour tests.

    [Fact]
    public async Task UpdateAsync_WhenUnauthorized_DoesNotInvalidateCache()
    {
        var databaseOptions = new DbContextOptionsBuilder<ResumeDbContext>()
            .UseInMemoryDatabase($"ResumeAppTests_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ResumeDbContext(databaseOptions);

        dbContext.Resumes.Add(new Resume
        {
            Id = 1,
            Name = "Protected User",
            Title = ".NET Developer",
            Email = "owner@example.com",
            Url = "https://example.com",
            Stack = "C#, ASP.NET Core",
            Country = "Nigeria",
            Summary = "Protected resume.",
            UserId = "owner-user-id",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();

        IDistributedCache cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));

        await cache.SetStringAsync(
            "resumes:all",
            "[{\"id\":1,\"name\":\"Protected User\"}]");

        await cache.SetStringAsync(
            "resume:1",
            "{\"id\":1,\"name\":\"Protected User\"}");

        var service = new ResumeService(dbContext, cache);

        var updateDto = new UpdateResumeDto
        {
            Name = "Unauthorized Change",
            Title = "Changed Title",
            Email = "changed@example.com",
            Url = "https://changed.example.com",
            Stack = "React",
            Country = "Nigeria",
            Summary = "This change must not happen."
        };

        var result = await service.UpdateAsync(
            id: 1,
            dto: updateDto,
            userId: "different-user-id");

        Assert.False(result);

        Assert.NotNull(await cache.GetStringAsync("resumes:all"));
        Assert.NotNull(await cache.GetStringAsync("resume:1"));

        var unchangedResume = await dbContext.Resumes.FindAsync(1);

        Assert.NotNull(unchangedResume);
        Assert.Equal("Protected User", unchangedResume.Name);
    }


    [Fact]
    public async Task UpdateAsync_WhenResumeDoesNotExist_ReturnsFalse()
    {
        var databaseOptions = new DbContextOptionsBuilder<ResumeDbContext>()
            .UseInMemoryDatabase($"ResumeAppTests_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ResumeDbContext(databaseOptions);

        IDistributedCache cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));

        await cache.SetStringAsync(
            "resumes:all",
            "[]");

        var service = new ResumeService(dbContext, cache);

        var updateDto = new UpdateResumeDto
        {
            Name = "Missing Resume",
            Title = ".NET Developer",
            Email = "missing@example.com",
            Url = "https://example.com",
            Stack = "C#, ASP.NET Core",
            Country = "Nigeria",
            Summary = "This resume does not exist."
        };

        var result = await service.UpdateAsync(
            id: 999,
            dto: updateDto,
            userId: "owner-user-id");

        Assert.False(result);

        Assert.Null(await dbContext.Resumes.FindAsync(999));

        // No update happened, so the existing list cache must remain.
        Assert.NotNull(await cache.GetStringAsync("resumes:all"));
    }


    [Fact]
    public async Task DeleteAsync_WhenResumeDoesNotExist_ReturnsFalse()
    {
        var databaseOptions = new DbContextOptionsBuilder<ResumeDbContext>()
            .UseInMemoryDatabase($"ResumeAppTests_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ResumeDbContext(databaseOptions);

        IDistributedCache cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));

        await cache.SetStringAsync(
            "resumes:all",
            "[]");

        var service = new ResumeService(dbContext, cache);

        var result = await service.DeleteAsync(
            id: 999,
            userId: "owner-user-id");

        Assert.False(result);

        Assert.Null(await dbContext.Resumes.FindAsync(999));

        // No deletion happened, so the existing list cache must remain.
        Assert.NotNull(await cache.GetStringAsync("resumes:all"));
    }


    [Fact]
    public async Task GetAllAsync_WhenDatabaseIsEmpty_ReturnsEmptyListAndCachesIt()
    {
        var databaseOptions = new DbContextOptionsBuilder<ResumeDbContext>()
            .UseInMemoryDatabase($"ResumeAppTests_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new ResumeDbContext(databaseOptions);

        IDistributedCache cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));

        var service = new ResumeService(dbContext, cache);

        var result = (await service.GetAllAsync()).ToList();

        Assert.Empty(result);

        var cachedJson = await cache.GetStringAsync("resumes:all");

        Assert.NotNull(cachedJson);

        var cachedResumes =
            JsonSerializer.Deserialize<List<ResumeResponseDto>>(cachedJson);

        Assert.NotNull(cachedResumes);
        Assert.Empty(cachedResumes);
    }
}