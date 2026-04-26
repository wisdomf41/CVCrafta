using Microsoft.AspNetCore.Mvc;
using ResumeApp.Server.DTOs.Resume;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumeController : ControllerBase
    {
        private readonly IResumeService _resumeService;

        public ResumeController(IResumeService resumeService)
        {
            _resumeService = resumeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResumeResponseDto>>> GetResumes()
        {
            var resumes = await _resumeService.GetAllAsync();
            return Ok(resumes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResumeResponseDto>> GetResume(int id)
        {
            var resume = await _resumeService.GetByIdAsync(id);

            if (resume == null)
            {
                return NotFound();
            }

            return Ok(resume);
        }

        [HttpPost]
        public async Task<ActionResult<ResumeResponseDto>> CreateResume(CreateResumeDto dto)
        {
            var createdResume = await _resumeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetResume), new { id = createdResume.Id }, createdResume);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResume(int id, UpdateResumeDto dto)
        {
            var updated = await _resumeService.UpdateAsync(id, dto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResume(int id)
        {
            var deleted = await _resumeService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}

