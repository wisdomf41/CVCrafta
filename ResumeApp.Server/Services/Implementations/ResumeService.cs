using Microsoft.EntityFrameworkCore;
using ResumeApp.Server.Data;
using ResumeApp.Server.DTOs.Resume;
using ResumeApp.Server.Model;
using ResumeApp.Server.Services.Interfaces;



namespace ResumeApp.Server.Services.Implementations
    {
        public class ResumeService : IResumeService
        {
            private readonly ResumeDbContext _resDb;

            public ResumeService(ResumeDbContext resDb)
            {
                _resDb = resDb;
            }

            public async Task<IEnumerable<ResumeResponseDto>> GetAllAsync()
            {
            return await _resDb.Resumes
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
            }

            public async Task<ResumeResponseDto?> GetByIdAsync(int id)
            {
                return await _resDb.Resumes
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
            }

            public async Task<ResumeResponseDto> CreateAsync(CreateResumeDto dto)
            {
                var resume = new Resume
                {
                    Name = dto.Name,
                    Title = dto.Title,
                    Email = dto.Email,
                    Url = dto.Url,
                    Stack = dto.Stack,
                    Country = dto.Country,
                    Summary = dto.Summary,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _resDb.Resumes.Add(resume);
                await _resDb.SaveChangesAsync();

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

            public async Task<bool> UpdateAsync(int id, UpdateResumeDto dto)
            {
                var existingResume = await _resDb.Resumes.FindAsync(id);

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
                return true;
            }

            public async Task<bool> DeleteAsync(int id)
            {
                var resume = await _resDb.Resumes.FindAsync(id);

                if (resume == null)
                {
                    return false;
                }

                _resDb.Resumes.Remove(resume);
                await _resDb.SaveChangesAsync();
                return true;
            }
        }
}

