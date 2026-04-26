using Microsoft.AspNetCore.Mvc;
using ResumeApp.Server.Data;
using ResumeApp.Server.DTOs.Skill;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public SkillController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SkillResponseDto>>> GetSkills()
        {
            var skills = await _skillService.GetAllAsync();
            return Ok(skills);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SkillResponseDto>> GetSkill(int id)
        {
            var skill = await _skillService.GetByIdAsync(id);
            if (skill == null) return NotFound();

            return Ok(skill);
        }

        [HttpGet("resume/{resumeId}")]
        public async Task<ActionResult<IEnumerable<SkillResponseDto>>> GetSkillsByResume(int resumeId)
        {
            var skills = await _skillService.GetByResumeIdAsync(resumeId);
            return Ok(skills);
        }

        [HttpPost]
        public async Task<ActionResult<SkillResponseDto>> CreateSkill(CreateSkillDto dto)
        {
            var createdSkill = await _skillService.CreateAsync(dto);
            if (createdSkill == null) return BadRequest("Invalid ResumeId.");

            return CreatedAtAction(nameof(GetSkill), new { id = createdSkill.Id }, createdSkill);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSkill(int id, UpdateSkillDto dto)
        {
            var updated = await _skillService.UpdateAsync(id, dto);
            if (!updated) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var deleted = await _skillService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}

