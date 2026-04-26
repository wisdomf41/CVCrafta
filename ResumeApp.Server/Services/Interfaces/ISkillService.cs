using ResumeApp.Server.DTOs.Skill;

namespace ResumeApp.Server.Services.Interfaces
{
    public interface ISkillService
    {
        Task<IEnumerable<SkillResponseDto>> GetAllAsync();
        Task<SkillResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<SkillResponseDto>> GetByResumeIdAsync(int resumeId);
        Task<SkillResponseDto?> CreateAsync(CreateSkillDto dto);
        Task<bool> UpdateAsync(int id, UpdateSkillDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
