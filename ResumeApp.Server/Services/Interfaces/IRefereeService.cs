using ResumeApp.Server.DTOs.Referee;

namespace ResumeApp.Server.Services.Interfaces
{
    public interface IRefereeService
    {
        Task<IEnumerable<RefereeResponseDto>> GetAllAsync();
        Task<RefereeResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<RefereeResponseDto>> GetByResumeIdAsync(int resumeId);
        Task<RefereeResponseDto?> CreateAsync(CreateRefereeDto dto);
        Task<bool> UpdateAsync(int id, UpdateRefereeDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
