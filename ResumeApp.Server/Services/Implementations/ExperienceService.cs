using Microsoft.EntityFrameworkCore;
using ResumeApp.Server.Data;
using ResumeApp.Server.DTOs.Experience;
using ResumeApp.Server.Model;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Services.Implementations
{
    public class ExperienceService : IExperienceService
    {

            private readonly ResumeDbContext _resDb;

            public ExperienceService(ResumeDbContext resDb)
            {
                _resDb = resDb;
            }

            public async Task<IEnumerable<ExperienceResponseDto>> GetAllAsync()
            {
            return await _resDb.Experiences
                .Select(e => new ExperienceResponseDto
                {
                    Id = e.Id,
                    Role = e.Role,
                    Company = e.Company,
                    Duration = e.Duration,
                    Description = e.Description,
                    ResumeId = e.ResumeId
                })
                .ToListAsync();
            }

            public async Task<ExperienceResponseDto?> GetByIdAsync(int id)
            {
                return await _resDb.Experiences
                    .Where(e => e.Id == id)
                    .Select(e => new ExperienceResponseDto
                    {
                        Id = e.Id,
                        Role = e.Role,
                        Company = e.Company,
                        Duration = e.Duration,
                        Description = e.Description,
                        ResumeId = e.ResumeId
                    })
                    .FirstOrDefaultAsync();
            }

            public async Task<IEnumerable<ExperienceResponseDto>> GetByResumeIdAsync(int resumeId)
            {
                return await _resDb.Experiences
                    .Where(e => e.ResumeId == resumeId)
                    .Select(e => new ExperienceResponseDto
                    {
                        Id = e.Id,
                        Role = e.Role,
                        Company = e.Company,
                        Duration = e.Duration,
                        Description = e.Description,
                        ResumeId = e.ResumeId
                    })
                    .ToListAsync();
            }

            public async Task<ExperienceResponseDto?> CreateAsync(CreateExperienceDto dto)
            {
                var resumeExists = await _resDb.Resumes.AnyAsync(r => r.Id == dto.ResumeId);
                if (!resumeExists)
                {
                    return null;
                }

                var experience = new Experience
                {
                    Role = dto.Role,
                    Company = dto.Company,
                    Duration = dto.Duration,
                    Description = dto.Description,
                    ResumeId = dto.ResumeId
                };

                _resDb.Experiences.Add(experience);
                await _resDb.SaveChangesAsync();

                return new ExperienceResponseDto
                {
                    Id = experience.Id,
                    Role = experience.Role,
                    Company = experience.Company,
                    Duration = experience.Duration,
                    Description = experience.Description,
                    ResumeId = experience.ResumeId
                };
            }

            public async Task<bool> UpdateAsync(int id, UpdateExperienceDto dto)
            {
                var existingExperience = await _resDb.Experiences.FindAsync(id);
                if (existingExperience == null)
                {
                    return false;
                }

                var resumeExists = await _resDb.Resumes.AnyAsync(r => r.Id == dto.ResumeId);
                if (!resumeExists)
                {
                    return false;
                }

                existingExperience.Role = dto.Role;
                existingExperience.Company = dto.Company;
                existingExperience.Duration = dto.Duration;
                existingExperience.Description = dto.Description;
                existingExperience.ResumeId = dto.ResumeId;

                await _resDb.SaveChangesAsync();
                return true;
            }

            public async Task<bool> DeleteAsync(int id)
            {
                var experience = await _resDb.Experiences.FindAsync(id);
                if (experience == null)
                {
                    return false;
                }

                _resDb.Experiences.Remove(experience);
                await _resDb.SaveChangesAsync();
                return true;
            }
        
    }
}
