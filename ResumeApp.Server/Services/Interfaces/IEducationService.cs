using ResumeApp.Server.DTOs.Education;

namespace ResumeApp.Server.Services.Interfaces
{
    public interface IEducationService
    {
        Task<IEnumerable<EducationResponseDto>> GetAllAsync();
        Task<EducationResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<EducationResponseDto>> GetByResumeIdAsync(int resumeId);
        Task<EducationResponseDto?> CreateAsync(CreateEducationDto dto);
        Task<bool> UpdateAsync(int id, UpdateEducationDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
