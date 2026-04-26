using Microsoft.EntityFrameworkCore;
using ResumeApp.Server.Data;
using ResumeApp.Server.DTOs.Certification;
using ResumeApp.Server.Model;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Services.Implementations
{
    public class CertificationService : ICertificationService
    {
        private readonly ResumeDbContext _resDb;

        public CertificationService(ResumeDbContext resDb)
        {
            _resDb = resDb;
        }

        public async Task<IEnumerable<CertificationResponseDto>> GetAllAsync()
        {
            return await _resDb.Certifications
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new CertificationResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Issuer = c.Issuer,
                    YearObtained = c.YearObtained,
                    CredentialUrl = c.CredentialUrl,
                    DisplayOrder = c.DisplayOrder,
                    ResumeId = c.ResumeId
                })
                .ToListAsync();
        }

        public async Task<CertificationResponseDto?> GetByIdAsync(int id)
        {
            return await _resDb.Certifications
                .Where(c => c.Id == id)
                .Select(c => new CertificationResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Issuer = c.Issuer,
                    YearObtained = c.YearObtained,
                    CredentialUrl = c.CredentialUrl,
                    DisplayOrder = c.DisplayOrder,
                    ResumeId = c.ResumeId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CertificationResponseDto>> GetByResumeIdAsync(int resumeId)
        {
            return await _resDb.Certifications
                .Where(c => c.ResumeId == resumeId)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new CertificationResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Issuer = c.Issuer,
                    YearObtained = c.YearObtained,
                    CredentialUrl = c.CredentialUrl,
                    DisplayOrder = c.DisplayOrder,
                    ResumeId = c.ResumeId
                })
                .ToListAsync();
        }

        public async Task<CertificationResponseDto?> CreateAsync(CreateCertificationDto dto)
        {
            var resumeExists = await _resDb.Resumes.AnyAsync(r => r.Id == dto.ResumeId);
            if (!resumeExists) return null;

            var certification = new Certification
            {
                Name = dto.Name,
                Issuer = dto.Issuer,
                YearObtained = dto.YearObtained,
                CredentialUrl = dto.CredentialUrl,
                DisplayOrder = dto.DisplayOrder,
                ResumeId = dto.ResumeId
            };

            _resDb.Certifications.Add(certification);
            await _resDb.SaveChangesAsync();

            return new CertificationResponseDto
            {
                Id = certification.Id,
                Name = certification.Name,
                Issuer = certification.Issuer,
                YearObtained = certification.YearObtained,
                CredentialUrl = certification.CredentialUrl,
                DisplayOrder = certification.DisplayOrder,
                ResumeId = certification.ResumeId
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateCertificationDto dto)
        {
            var existingCertification = await _resDb.Certifications.FindAsync(id);
            if (existingCertification == null) return false;

            var resumeExists = await _resDb.Resumes.AnyAsync(r => r.Id == dto.ResumeId);
            if (!resumeExists) return false;

            existingCertification.Name = dto.Name;
            existingCertification.Issuer = dto.Issuer;
            existingCertification.YearObtained = dto.YearObtained;
            existingCertification.CredentialUrl = dto.CredentialUrl;
            existingCertification.DisplayOrder = dto.DisplayOrder;
            existingCertification.ResumeId = dto.ResumeId;

            await _resDb.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var certification = await _resDb.Certifications.FindAsync(id);
            if (certification == null) return false;

            _resDb.Certifications.Remove(certification);
            await _resDb.SaveChangesAsync();
            return true;
        }
    }
}
