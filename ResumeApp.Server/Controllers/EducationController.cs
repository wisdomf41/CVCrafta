using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeApp.Server.Data;
using ResumeApp.Server.DTOs.Education;
using ResumeApp.Server.Model;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EducationController : ControllerBase
    {
        private readonly IEducationService _educationService;

        public EducationController(IEducationService educationService)
        {
            _educationService = educationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EducationResponseDto>>> GetEducations()
        {
            var educations = await _educationService.GetAllAsync();
            return Ok(educations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EducationResponseDto>> GetEducation(int id)
        {
            var education = await _educationService.GetByIdAsync(id);
            if (education == null) return NotFound();

            return Ok(education);
        }

        [HttpGet("resume/{resumeId}")]
        public async Task<ActionResult<IEnumerable<EducationResponseDto>>> GetEducationsByResume(int resumeId)
        {
            var educations = await _educationService.GetByResumeIdAsync(resumeId);
            return Ok(educations);
        }

        [HttpPost]
        public async Task<ActionResult<EducationResponseDto>> CreateEducation(CreateEducationDto dto)
        {
            var createdEducation = await _educationService.CreateAsync(dto);
            if (createdEducation == null) return BadRequest("Invalid ResumeId.");

            return CreatedAtAction(nameof(GetEducation), new { id = createdEducation.Id }, createdEducation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEducation(int id, UpdateEducationDto dto)
        {
            var updated = await _educationService.UpdateAsync(id, dto);
            if (!updated) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEducation(int id)
        {
            var deleted = await _educationService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}


