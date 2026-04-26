using ResumeApp.Server.DTOs.Resume;

namespace ResumeApp.Server.Services.Interfaces
{
    public interface IResumeService
    {
        Task<IEnumerable<ResumeResponseDto>> GetAllAsync();
        Task<ResumeResponseDto?> GetByIdAsync(int id);
        Task<ResumeResponseDto> CreateAsync(CreateResumeDto dto);
        Task<bool> UpdateAsync(int id, UpdateResumeDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
