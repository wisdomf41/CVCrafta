using ResumeApp.Server.DTOs.Resume;

namespace ResumeApp.Server.Services.Interfaces
{
    public interface IResumeService
    {
        Task<IEnumerable<ResumeResponseDto>> GetAllAsync();
        Task<ResumeResponseDto?> GetByIdAsync(int id);

        // Get only the resume that belongs to the logged-in user
        // Returns null when the logged-in user has no resume yet.
        Task<ResumeResponseDto?> GetByUserIdAsync(string userId);

        Task<ResumeResponseDto> CreateAsync(CreateResumeDto dto, string userId);

        Task<bool> UpdateAsync(int id, UpdateResumeDto dto, string userId);
        Task<bool> DeleteAsync(int id, string userId);
    }
}
