using Microsoft.EntityFrameworkCore;
using ResumeApp.Server.Data;
using ResumeApp.Server.DTOs.Skill;
using ResumeApp.Server.Model;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Services.Implementations
{

        public class SkillService : ISkillService
        {
            private readonly ResumeDbContext _resDb;

            public SkillService(ResumeDbContext resDb)
            {
                _resDb = resDb;
            }

            public async Task<IEnumerable<SkillResponseDto>> GetAllAsync()
            {
            return await _resDb.Skills
                .OrderBy(s => s.DisplayOrder)
                .Select(s => new SkillResponseDto
                {
                    Id = s.Id,
                    Category = s.Category,
                    Skills = s.Skills,
                    DisplayOrder = s.DisplayOrder,
                    ResumeId = s.ResumeId
                })
                .ToListAsync();
            }

            public async Task<SkillResponseDto?> GetByIdAsync(int id)
            {
                return await _resDb.Skills
                    .Where(s => s.Id == id)
                    .Select(s => new SkillResponseDto
                    {
                        Id = s.Id,
                        Category = s.Category,
                        Skills = s.Skills,
                        DisplayOrder = s.DisplayOrder,
                        ResumeId = s.ResumeId
                    })
                    .FirstOrDefaultAsync();
            }

            public async Task<IEnumerable<SkillResponseDto>> GetByResumeIdAsync(int resumeId)
            {
                return await _resDb.Skills
                    .Where(s => s.ResumeId == resumeId)
                    .OrderBy(s => s.DisplayOrder)
                    .Select(s => new SkillResponseDto
                    {
                        Id = s.Id,
                        Category = s.Category,
                        Skills = s.Skills,
                        DisplayOrder = s.DisplayOrder,
                        ResumeId = s.ResumeId
                    })
                    .ToListAsync();
            }

            public async Task<SkillResponseDto?> CreateAsync(CreateSkillDto dto)
            {
                var resumeExists = await _resDb.Resumes.AnyAsync(r => r.Id == dto.ResumeId);
                if (!resumeExists) return null;

                var skill = new Skill
                {
                    Category = dto.Category,
                    Skills = dto.Skills,
                    DisplayOrder = dto.DisplayOrder,
                    ResumeId = dto.ResumeId
                };

                _resDb.Skills.Add(skill);
                await _resDb.SaveChangesAsync();

                return new SkillResponseDto
                {
                    Id = skill.Id,
                    Category = skill.Category,
                    Skills = skill.Skills,
                    DisplayOrder = skill.DisplayOrder,
                    ResumeId = skill.ResumeId
                };
            }

            public async Task<bool> UpdateAsync(int id, UpdateSkillDto dto)
            {
                var existingSkill = await _resDb.Skills.FindAsync(id);
                if (existingSkill == null) return false;

                var resumeExists = await _resDb.Resumes.AnyAsync(r => r.Id == dto.ResumeId);
                if (!resumeExists) return false;

                existingSkill.Category = dto.Category;
                existingSkill.Skills = dto.Skills;
                existingSkill.DisplayOrder = dto.DisplayOrder;
                existingSkill.ResumeId = dto.ResumeId;

                await _resDb.SaveChangesAsync();
                return true;
            }

            public async Task<bool> DeleteAsync(int id)
            {
                var skill = await _resDb.Skills.FindAsync(id);
                if (skill == null) return false;

                _resDb.Skills.Remove(skill);
                await _resDb.SaveChangesAsync();
                return true;
            }
        }
}
