using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeApp.Server.Data;
using ResumeApp.Server.DTOs.Interest;
using ResumeApp.Server.Model;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [NonController] // Temporarily excludes this unfinished feature from the published API.
    public class InterestController : ControllerBase
    {
        private readonly IInterestService _interestService;

        public InterestController(IInterestService interestService)
        {
            _interestService = interestService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InterestResponseDto>>> GetInterests()
        {
            var interests = await _interestService.GetAllAsync();
            return Ok(interests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InterestResponseDto>> GetInterest(int id)
        {
            var interest = await _interestService.GetByIdAsync(id);
            if (interest == null) return NotFound();

            return Ok(interest);
        }

        [HttpGet("resume/{resumeId}")]
        public async Task<ActionResult<IEnumerable<InterestResponseDto>>> GetInterestsByResume(int resumeId)
        {
            var interests = await _interestService.GetByResumeIdAsync(resumeId);
            return Ok(interests);
        }

        [HttpPost]
        public async Task<ActionResult<InterestResponseDto>> CreateInterest(CreateInterestDto dto)
        {
            var createdInterest = await _interestService.CreateAsync(dto);
            if (createdInterest == null) return BadRequest("Invalid ResumeId.");

            return CreatedAtAction(nameof(GetInterest), new { id = createdInterest.Id }, createdInterest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInterest(int id, UpdateInterestDto dto)
        {
            var updated = await _interestService.UpdateAsync(id, dto);
            if (!updated) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInterest(int id)
        {
            var deleted = await _interestService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}

