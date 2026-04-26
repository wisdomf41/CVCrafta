using ResumeApp.Server.DTOs.Experience;

namespace ResumeApp.Server.Services.Interfaces
{
    public interface IExperienceService
    {
        Task<IEnumerable<ExperienceResponseDto>> GetAllAsync();
        Task<ExperienceResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<ExperienceResponseDto>> GetByResumeIdAsync(int resumeId);
        Task<ExperienceResponseDto?> CreateAsync(CreateExperienceDto dto);
        Task<bool> UpdateAsync(int id, UpdateExperienceDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
