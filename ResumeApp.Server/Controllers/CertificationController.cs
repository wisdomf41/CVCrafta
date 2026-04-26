using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeApp.Server.Data;
using ResumeApp.Server.DTOs.Certification;
using ResumeApp.Server.Model;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificationController : ControllerBase
    {
        private readonly ICertificationService _certificationService;

        public CertificationController(ICertificationService certificationService)
        {
            _certificationService = certificationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CertificationResponseDto>>> GetCertifications()
        {
            var certifications = await _certificationService.GetAllAsync();
            return Ok(certifications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CertificationResponseDto>> GetCertification(int id)
        {
            var certification = await _certificationService.GetByIdAsync(id);
            if (certification == null) return NotFound();

            return Ok(certification);
        }

        [HttpGet("resume/{resumeId}")]
        public async Task<ActionResult<IEnumerable<CertificationResponseDto>>> GetCertificationsByResume(int resumeId)
        {
            var certifications = await _certificationService.GetByResumeIdAsync(resumeId);
            return Ok(certifications);
        }

        [HttpPost]
        public async Task<ActionResult<CertificationResponseDto>> CreateCertification(CreateCertificationDto dto)
        {
            var createdCertification = await _certificationService.CreateAsync(dto);
            if (createdCertification == null) return BadRequest("Invalid ResumeId.");

            return CreatedAtAction(nameof(GetCertification), new { id = createdCertification.Id }, createdCertification);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCertification(int id, UpdateCertificationDto dto)
        {
            var updated = await _certificationService.UpdateAsync(id, dto);
            if (!updated) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertification(int id)
        {
            var deleted = await _certificationService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}


