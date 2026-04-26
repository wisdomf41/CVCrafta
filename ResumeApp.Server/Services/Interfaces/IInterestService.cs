using ResumeApp.Server.DTOs.Interest;

namespace ResumeApp.Server.Services.Interfaces
{
    public interface IInterestService
    {
        Task<IEnumerable<InterestResponseDto>> GetAllAsync();
        Task<InterestResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<InterestResponseDto>> GetByResumeIdAsync(int resumeId);
        Task<InterestResponseDto?> CreateAsync(CreateInterestDto dto);
        Task<bool> UpdateAsync(int id, UpdateInterestDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
