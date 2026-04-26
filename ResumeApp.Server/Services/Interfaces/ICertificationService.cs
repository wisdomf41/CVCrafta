using ResumeApp.Server.DTOs.Certification;

namespace ResumeApp.Server.Services.Interfaces
{
    public interface ICertificationService
    {
        Task<IEnumerable<CertificationResponseDto>> GetAllAsync();
        Task<CertificationResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<CertificationResponseDto>> GetByResumeIdAsync(int resumeId);
        Task<CertificationResponseDto?> CreateAsync(CreateCertificationDto dto);
        Task<bool> UpdateAsync(int id, UpdateCertificationDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
