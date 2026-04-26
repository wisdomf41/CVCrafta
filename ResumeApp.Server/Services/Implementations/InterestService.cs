using Microsoft.EntityFrameworkCore;
using ResumeApp.Server.Data;
using ResumeApp.Server.DTOs.Interest;
using ResumeApp.Server.Model;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Services.Implementations
{
    public class InterestService : IInterestService
    {
        private readonly ResumeDbContext _resDb;

        public InterestService(ResumeDbContext resDb)
        {
            _resDb = resDb;
        }

        public async Task<IEnumerable<InterestResponseDto>> GetAllAsync()
        {
            return await _resDb.Interests
                .OrderBy(i => i.DisplayOrder)
                .Select(i => new InterestResponseDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    DisplayOrder = i.DisplayOrder,
                    ResumeId = i.ResumeId
                })
                .ToListAsync();
        }

        public async Task<InterestResponseDto?> GetByIdAsync(int id)
        {
            return await _resDb.Interests
                .Where(i => i.Id == id)
                .Select(i => new InterestResponseDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    DisplayOrder = i.DisplayOrder,
                    ResumeId = i.ResumeId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<InterestResponseDto>> GetByResumeIdAsync(int resumeId)
        {
            return await _resDb.Interests
                .Where(i => i.ResumeId == resumeId)
                .OrderBy(i => i.DisplayOrder)
                .Select(i => new InterestResponseDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    DisplayOrder = i.DisplayOrder,
                    ResumeId = i.ResumeId
                })
                .ToListAsync();
        }

        public async Task<InterestResponseDto?> CreateAsync(CreateInterestDto dto)
        {
            var resumeExists = await _resDb.Resumes.AnyAsync(r => r.Id == dto.ResumeId);
            if (!resumeExists) return null;

            var interest = new Interest
            {
                Title = dto.Title,
                Description = dto.Description,
                DisplayOrder = dto.DisplayOrder,
                ResumeId = dto.ResumeId
            };

            _resDb.Interests.Add(interest);
            await _resDb.SaveChangesAsync();

            return new InterestResponseDto
            {
                Id = interest.Id,
                Title = interest.Title,
                Description = interest.Description,
                DisplayOrder = interest.DisplayOrder,
                ResumeId = interest.ResumeId
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateInterestDto dto)
        {
            var existingInterest = await _resDb.Interests.FindAsync(id);
            if (existingInterest == null) return false;

            var resumeExists = await _resDb.Resumes.AnyAsync(r => r.Id == dto.ResumeId);
            if (!resumeExists) return false;

            existingInterest.Title = dto.Title;
            existingInterest.Description = dto.Description;
            existingInterest.DisplayOrder = dto.DisplayOrder;
            existingInterest.ResumeId = dto.ResumeId;

            await _resDb.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var interest = await _resDb.Interests.FindAsync(id);
            if (interest == null) return false;

            _resDb.Interests.Remove(interest);
            await _resDb.SaveChangesAsync();
            return true;
        }
    }
}
