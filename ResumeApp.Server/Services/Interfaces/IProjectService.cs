using ResumeApp.Server.DTOs.Project;

namespace ResumeApp.Server.Services.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectResponseDto>> GetAllAsync();
        Task<ProjectResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProjectResponseDto>> GetByResumeIdAsync(int resumeId);
        Task<ProjectResponseDto?> CreateAsync(CreateProjectDto dto);
        Task<bool> UpdateAsync(int id, UpdateProjectDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
