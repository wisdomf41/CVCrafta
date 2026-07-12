using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeApp.Server.DTOs.Resume;
using ResumeApp.Server.Services.Interfaces;
using System.Security.Claims;

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


        [Authorize]
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<ResumeResponseDto>>> GetMyResumes()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }
            var resumes = await _resumeService.GetByUserIdAsync(userId);
            if (resumes == null)
            {
                return NotFound("No resumes found for the current user.");
            }
            return Ok(resumes);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResumeResponseDto>> GetResume(int id)
        {
            var resume = await _resumeService.GetByIdAsync(id);

            if (resume == null)
            {
                return NotFound();
            }

            return Ok(resume);
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ResumeResponseDto>> CreateResume(CreateResumeDto dto)
        {
            //validate user id from token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var createdResume = await _resumeService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetResume), new { id = createdResume.Id }, createdResume);
        }


        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateResume(int id, UpdateResumeDto dto)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userid))
            {
                return Unauthorized("User ID not found in token");
            }

            var updated = await _resumeService.UpdateAsync(id, dto, userid);

            if (!updated)
            {
                return NotFound("Resume not found or you do not have permission to update it.");
            }

            return NoContent();
        }


        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteResume(int id)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userid))
            { 
                return Unauthorized("User ID not found in token.") ;
            }

            var deleted = await _resumeService.DeleteAsync(id, userid);

            if (!deleted)
            {
                return NotFound("Resume not found or you do not have permission to delete it.");
            }

            return NoContent();
        }
    }
}

