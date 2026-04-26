using Microsoft.EntityFrameworkCore;
using ResumeApp.Server.Data;
using ResumeApp.Server.DTOs.Referee;
using ResumeApp.Server.Model;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Services.Implementations
{
    public class RefereeService : IRefereeService
    {
        private readonly ResumeDbContext _resDb;

        public RefereeService(ResumeDbContext resDb)
        {
            _resDb = resDb;
        }

        public async Task<IEnumerable<RefereeResponseDto>> GetAllAsync()
        {
            return await _resDb.Referees
                .OrderBy(r => r.DisplayOrder)
                .Select(r => new RefereeResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Title = r.Title,
                    Company = r.Company,
                    Email = r.Email,
                    Phone = r.Phone,
                    Notes = r.Notes,
                    DisplayOrder = r.DisplayOrder,
                    ResumeId = r.ResumeId
                })
                .ToListAsync();
        }

        public async Task<RefereeResponseDto?> GetByIdAsync(int id)
        {
            return await _resDb.Referees
                .Where(r => r.Id == id)
                .Select(r => new RefereeResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Title = r.Title,
                    Company = r.Company,
                    Email = r.Email,
                    Phone = r.Phone,
                    Notes = r.Notes,
                    DisplayOrder = r.DisplayOrder,
                    ResumeId = r.ResumeId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<RefereeResponseDto>> GetByResumeIdAsync(int resumeId)
        {
            return await _resDb.Referees
                .Where(r => r.ResumeId == resumeId)
                .OrderBy(r => r.DisplayOrder)
                .Select(r => new RefereeResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Title = r.Title,
                    Company = r.Company,
                    Email = r.Email,
                    Phone = r.Phone,
                    Notes = r.Notes,
                    DisplayOrder = r.DisplayOrder,
                    ResumeId = r.ResumeId
                })
                .ToListAsync();
        }

        public async Task<RefereeResponseDto?> CreateAsync(CreateRefereeDto dto)
        {
            var resumeExists = await _resDb.Resumes.AnyAsync(r => r.Id == dto.ResumeId);
            if (!resumeExists) return null;

            var referee = new Referee
            {
                Name = dto.Name,
                Title = dto.Title,
                Company = dto.Company,
                Email = dto.Email,
                Phone = dto.Phone,
                Notes = dto.Notes,
                DisplayOrder = dto.DisplayOrder,
                ResumeId = dto.ResumeId
            };

            _resDb.Referees.Add(referee);
            await _resDb.SaveChangesAsync();

            return new RefereeResponseDto
            {
                Id = referee.Id,
                Name = referee.Name,
                Title = referee.Title,
                Company = referee.Company,
                Email = referee.Email,
                Phone = referee.Phone,
                Notes = referee.Notes,
                DisplayOrder = referee.DisplayOrder,
                ResumeId = referee.ResumeId
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateRefereeDto dto)
        {
            var existingReferee = await _resDb.Referees.FindAsync(id);
            if (existingReferee == null) return false;

            var resumeExists = await _resDb.Resumes.AnyAsync(r => r.Id == dto.ResumeId);
            if (!resumeExists) return false;

            existingReferee.Name = dto.Name;
            existingReferee.Title = dto.Title;
            existingReferee.Company = dto.Company;
            existingReferee.Email = dto.Email;
            existingReferee.Phone = dto.Phone;
            existingReferee.Notes = dto.Notes;
            existingReferee.DisplayOrder = dto.DisplayOrder;
            existingReferee.ResumeId = dto.ResumeId;

            await _resDb.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var referee = await _resDb.Referees.FindAsync(id);
            if (referee == null) return false;

            _resDb.Referees.Remove(referee);
            await _resDb.SaveChangesAsync();
            return true;
        }
    }
}
