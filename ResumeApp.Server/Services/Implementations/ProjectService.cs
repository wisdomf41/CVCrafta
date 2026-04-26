using Microsoft.EntityFrameworkCore;
using ResumeApp.Server.Data;
using ResumeApp.Server.DTOs.Project;
using ResumeApp.Server.Model;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Services.Implementations
{
        public class ProjectService : IProjectService
        {
            private readonly ResumeDbContext _resDb;

            public ProjectService(ResumeDbContext resDb)
            {
                _resDb = resDb;
            }

            public async Task<IEnumerable<ProjectResponseDto>> GetAllAsync()
            {
            return await _resDb.Projects
                .OrderBy(p => p.DisplayOrder)
                .Select(p => new ProjectResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Technologies = p.Technologies,
                    Description = p.Description,
                    ProjectUrl = p.ProjectUrl,
                    GithubUrl = p.GithubUrl,
                    DisplayOrder = p.DisplayOrder,
                    ResumeId = p.ResumeId
                })
                .ToListAsync();
            }

            public async Task<ProjectResponseDto?> GetByIdAsync(int id)
            {
                return await _resDb.Projects
                    .Where(p => p.Id == id)
                    .Select(p => new ProjectResponseDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Technologies = p.Technologies,
                        Description = p.Description,
                        ProjectUrl = p.ProjectUrl,
                        GithubUrl = p.GithubUrl,
                        DisplayOrder = p.DisplayOrder,
                        ResumeId = p.ResumeId
                    })
                    .FirstOrDefaultAsync();
            }

            public async Task<IEnumerable<ProjectResponseDto>> GetByResumeIdAsync(int resumeId)
            {
                return await _resDb.Projects
                    .Where(p => p.ResumeId == resumeId)
                    .OrderBy(p => p.DisplayOrder)
                    .Select(p => new ProjectResponseDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Technologies = p.Technologies,
                        Description = p.Description,
                        ProjectUrl = p.ProjectUrl,
                        GithubUrl = p.GithubUrl,
                        DisplayOrder = p.DisplayOrder,
                        ResumeId = p.ResumeId
                    })
                    .ToListAsync();
            }

            public async Task<ProjectResponseDto?> CreateAsync(CreateProjectDto dto)
            {
                var resumeExists = await _resDb.Resumes.AnyAsync(r => r.Id == dto.ResumeId);
                if (!resumeExists) return null;

                var project = new Project
                {
                    Name = dto.Name,
                    Technologies = dto.Technologies,
                    Description = dto.Description,
                    ProjectUrl = dto.ProjectUrl,
                    GithubUrl = dto.GithubUrl,
                    DisplayOrder = dto.DisplayOrder,
                    ResumeId = dto.ResumeId
                };

                _resDb.Projects.Add(project);
                await _resDb.SaveChangesAsync();

                return new ProjectResponseDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    Technologies = project.Technologies,
                    Description = project.Description,
                    ProjectUrl = project.ProjectUrl,
                    GithubUrl = project.GithubUrl,
                    DisplayOrder = project.DisplayOrder,
                    ResumeId = project.ResumeId
                };
            }

            public async Task<bool> UpdateAsync(int id, UpdateProjectDto dto)
            {
                var existingProject = await _resDb.Projects.FindAsync(id);
                if (existingProject == null) return false;

                var resumeExists = await _resDb.Resumes.AnyAsync(r => r.Id == dto.ResumeId);
                if (!resumeExists) return false;

                existingProject.Name = dto.Name;
                existingProject.Technologies = dto.Technologies;
                existingProject.Description = dto.Description;
                existingProject.ProjectUrl = dto.ProjectUrl;
                existingProject.GithubUrl = dto.GithubUrl;
                existingProject.DisplayOrder = dto.DisplayOrder;
                existingProject.ResumeId = dto.ResumeId;

                await _resDb.SaveChangesAsync();
                return true;
            }

            public async Task<bool> DeleteAsync(int id)
            {
                var project = await _resDb.Projects.FindAsync(id);
                if (project == null) return false;

                _resDb.Projects.Remove(project);
                await _resDb.SaveChangesAsync();
                return true;
            }
        }
}
