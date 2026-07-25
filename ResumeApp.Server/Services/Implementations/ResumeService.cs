using Microsoft.EntityFrameworkCore;
using ResumeApp.Server.Data;
using ResumeApp.Server.DTOs.Resume;
using ResumeApp.Server.Model;
using ResumeApp.Server.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;



namespace ResumeApp.Server.Services.Implementations
    {
        public class ResumeService : IResumeService
        {
            private readonly ResumeDbContext _resDb;
            private readonly IDistributedCache _cache;
            private const string AllResumesCacheKey = "resumes:all";

        public ResumeService(ResumeDbContext resDb, IDistributedCache cache)
            {
                _resDb = resDb;
                _cache = cache;
            }

        public async Task<IEnumerable<ResumeResponseDto>> GetAllAsync()
        {
            // First check Redis for an existing cached copy.
            var cachedResumes = await _cache.GetStringAsync(AllResumesCacheKey);

            // If Redis contains the data, return it instead of querying SQL Server.
            if (!string.IsNullOrWhiteSpace(cachedResumes))
            {
                var resumesFromCache =
                    JsonSerializer.Deserialize<List<ResumeResponseDto>>(cachedResumes);

                if (resumesFromCache != null)
                {
                    return resumesFromCache;
                }
            }

            // EXISTING DATABASE QUERY:
            // Redis did not have the data, so now we ask SQL Server.
            var resumes = await _resDb.Resumes
                .Select(r => new ResumeResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Title = r.Title,
                    Email = r.Email,
                    Url = r.Url,
                    Stack = r.Stack,
                    Country = r.Country,
                    Summary = r.Summary,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .ToListAsync();

            // Convert the result into JSON so Redis can store it.
            var resumesJson = JsonSerializer.Serialize(resumes);

            // Keep this cached copy for 5 minutes.
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            // Save the result in Redis.
            await _cache.SetStringAsync(
                AllResumesCacheKey,
                resumesJson,
                cacheOptions);

            return resumes;
        }

        public async Task<ResumeResponseDto?> GetByIdAsync(int id)
            {
            // First check Redis for an existing cached copy by ID.
            var cacheKey = $"resume:{id}";

            var cachedResume = await _cache.GetStringAsync(cacheKey);

            // If Redis contains the data, return it instead of querying SQL Server.
            if (!string.IsNullOrWhiteSpace(cachedResume))
            {
                var resumeFromCache = JsonSerializer.Deserialize<ResumeResponseDto>(cachedResume);

                if (resumeFromCache != null)
                {
                    return resumeFromCache;
                }
            }

            var resume = await _resDb.Resumes
                    .Where(r => r.Id == id)
                    .Select(r => new ResumeResponseDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Title = r.Title,
                        Email = r.Email,
                        Url = r.Url,
                        Stack = r.Stack,
                        Country = r.Country,
                        Summary = r.Summary,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if(resume== null)
            {
                return null;
            }

            // Convert the result into JSON so Redis can store it.
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(resume),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return resume;
        }

        public async Task<ResumeResponseDto?> GetByUserIdAsync(string userId)
            {
                return await _resDb.Resumes
                    .Where(r => r.UserId == userId)
                    .Select(r => new ResumeResponseDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Title = r.Title,
                        Email = r.Email,
                        Url = r.Url,
                        Stack = r.Stack,
                        Country = r.Country,
                        Summary = r.Summary,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt
                    })
                    .FirstOrDefaultAsync();
            }


            public async Task<ResumeResponseDto> CreateAsync(CreateResumeDto dto, string userId)
            {
                var resume = new Resume
                {
                    Name = dto.Name,
                    Title = dto.Title,
                    Email = dto.Email,
                    Url = dto.Url,
                    Stack = dto.Stack,
                    Country = dto.Country,
                    UserId = userId,
                    Summary = dto.Summary,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _resDb.Resumes.Add(resume);
                await _resDb.SaveChangesAsync();
                await _cache.RemoveAsync(AllResumesCacheKey); // Invalidate the old cache after creating a new resume.

            return new ResumeResponseDto
                {
                    Id = resume.Id,
                    Name = resume.Name,
                    Title = resume.Title,
                    Email = resume.Email,
                    Url = resume.Url,
                    Stack = resume.Stack,
                    Country = resume.Country,
                    Summary = resume.Summary,
                    CreatedAt = resume.CreatedAt,
                    UpdatedAt = resume.UpdatedAt
                };
            }

        public async Task<bool> UpdateAsync(int id, UpdateResumeDto dto, string userId)
        {
            var existingResume = await _resDb.Resumes
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (existingResume == null)
            {
                return false;
            }

            existingResume.Name = dto.Name;
            existingResume.Title = dto.Title;
            existingResume.Email = dto.Email;
            existingResume.Url = dto.Url;
            existingResume.Stack = dto.Stack;
            existingResume.Country = dto.Country;
            existingResume.Summary = dto.Summary;
            existingResume.UpdatedAt = DateTime.UtcNow;

            await _resDb.SaveChangesAsync();
            await _cache.RemoveAsync(AllResumesCacheKey); // Invalidate the old cache after updating a resume.
            await _cache.RemoveAsync($"resume:{id}");
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string userId)
            {
            var resume = await _resDb.Resumes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

                if (resume == null)
                {
                    return false;
                }

                _resDb.Resumes.Remove(resume);
                await _resDb.SaveChangesAsync();
                await _cache.RemoveAsync(AllResumesCacheKey); // Invalidate the old cache after deleting a resume.
                await _cache.RemoveAsync($"resume:{id}");
            return true;
            }
        }
}

