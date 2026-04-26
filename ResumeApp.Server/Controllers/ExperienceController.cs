using Microsoft.AspNetCore.Mvc;
using ResumeApp.Server.DTOs.Experience;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExperienceController : ControllerBase
    {
        private readonly IExperienceService _experienceService;

        public ExperienceController(IExperienceService experienceService)
        {
            _experienceService = experienceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExperienceResponseDto>>> GetExperiences()
        {
            var experiences = await _experienceService.GetAllAsync();
            return Ok(experiences);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExperienceResponseDto>> GetExperience(int id)
        {
            var experience = await _experienceService.GetByIdAsync(id);

            if (experience == null)
            {
                return NotFound();
            }

            return Ok(experience);
        }

        [HttpGet("resume/{resumeId}")]
        public async Task<ActionResult<IEnumerable<ExperienceResponseDto>>> GetExperiencesByResume(int resumeId)
        {
            var experiences = await _experienceService.GetByResumeIdAsync(resumeId);
            return Ok(experiences);
        }

        [HttpPost]
        public async Task<ActionResult<ExperienceResponseDto>> CreateExperience(CreateExperienceDto dto)
        {
            var createdExperience = await _experienceService.CreateAsync(dto);

            if (createdExperience == null)
            {
                return BadRequest("Invalid ResumeId.");
            }

            return CreatedAtAction(nameof(GetExperience), new { id = createdExperience.Id }, createdExperience);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExperience(int id, UpdateExperienceDto dto)
        {
            var updated = await _experienceService.UpdateAsync(id, dto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExperience(int id)
        {
            var deleted = await _experienceService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }

}


