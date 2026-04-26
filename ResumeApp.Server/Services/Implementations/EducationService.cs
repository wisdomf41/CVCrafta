using Microsoft.EntityFrameworkCore;
using ResumeApp.Server.Data;
using ResumeApp.Server.DTOs.Education;
using ResumeApp.Server.Model;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Services.Implementations
{
    public class EducationService : IEducationService
    {
        private readonly ResumeDbContext _resDb;

        public EducationService(ResumeDbContext resDb)
        {
            _resDb = resDb;
        }

        public async Task<IEnumerable<EducationResponseDto>> GetAllAsync()
        {
            return await _resDb.Educations
                .OrderBy(e => e.DisplayOrder)
                .Select(e => new EducationResponseDto
                {
                    Id = e.Id,
                    Institution = e.Institution,
                    Degree = e.Degree,
                    FieldOfStudy = e.FieldOfStudy,
                    Grade = e.Grade,
                    YearGraduated = e.YearGraduated,
                    DisplayOrder = e.DisplayOrder,
                    ResumeId = e.ResumeId
                })
                .ToListAsync();
        }

        public async Task<EducationResponseDto?> GetByIdAsync(int id)
        {
            return await _resDb.Educations
                .Where(e => e.Id == id)
                .Select(e => new EducationResponseDto
                {
                    Id = e.Id,
                    Institution = e.Institution,
                    Degree = e.Degree,
                    FieldOfStudy = e.FieldOfStudy,
                    Grade = e.Grade,
                    YearGraduated = e.YearGraduated,
                    DisplayOrder = e.DisplayOrder,
                    ResumeId = e.ResumeId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<EducationResponseDto>> GetByResumeIdAsync(int resumeId)
        {
            return await _resDb.Educations
                .Where(e => e.ResumeId == resumeId)
                .OrderBy(e => e.DisplayOrder)
                .Select(e => new EducationResponseDto
                {
                    Id = e.Id,
                    Institution = e.Institution,
                    Degree = e.Degree,
                    FieldOfStudy = e.FieldOfStudy,
                    Grade = e.Grade,
                    YearGraduated = e.YearGraduated,
                    DisplayOrder = e.DisplayOrder,
                    ResumeId = e.ResumeId
                })
                .ToListAsync();
        }

        public async Task<EducationResponseDto?> CreateAsync(CreateEducationDto dto)
        {
            var resumeExists = await _resDb.Resumes.AnyAsync(r => r.Id == dto.ResumeId);
            if (!resumeExists) return null;

            var education = new Education
            {
                Institution = dto.Institution,
                Degree = dto.Degree,
                FieldOfStudy = dto.FieldOfStudy,
                Grade = dto.Grade,
                YearGraduated = dto.YearGraduated,
                DisplayOrder = dto.DisplayOrder,
                ResumeId = dto.ResumeId
            };

            _resDb.Educations.Add(education);
            await _resDb.SaveChangesAsync();

            return new EducationResponseDto
            {
                Id = education.Id,
                Institution = education.Institution,
                Degree = education.Degree,
                FieldOfStudy = education.FieldOfStudy,
                Grade = education.Grade,
                YearGraduated = education.YearGraduated,
                DisplayOrder = education.DisplayOrder,
                ResumeId = education.ResumeId
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateEducationDto dto)
        {
            var existingEducation = await _resDb.Educations.FindAsync(id);
            if (existingEducation == null) return false;

            var resumeExists = await _resDb.Resumes.AnyAsync(r => r.Id == dto.ResumeId);
            if (!resumeExists) return false;

            existingEducation.Institution = dto.Institution;
            existingEducation.Degree = dto.Degree;
            existingEducation.FieldOfStudy = dto.FieldOfStudy;
            existingEducation.Grade = dto.Grade;
            existingEducation.YearGraduated = dto.YearGraduated;
            existingEducation.DisplayOrder = dto.DisplayOrder;
            existingEducation.ResumeId = dto.ResumeId;

            await _resDb.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var education = await _resDb.Educations.FindAsync(id);
            if (education == null) return false;

            _resDb.Educations.Remove(education);
            await _resDb.SaveChangesAsync();
            return true;
        }
    }
}
