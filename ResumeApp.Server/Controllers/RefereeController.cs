using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeApp.Server.Data;
using ResumeApp.Server.DTOs.Referee;
using ResumeApp.Server.Model;
using ResumeApp.Server.Services.Interfaces;

namespace ResumeApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefereeController : ControllerBase
    {
        private readonly IRefereeService _refereeService;

        public RefereeController(IRefereeService refereeService)
        {
            _refereeService = refereeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RefereeResponseDto>>> GetReferees()
        {
            var referees = await _refereeService.GetAllAsync();
            return Ok(referees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RefereeResponseDto>> GetReferee(int id)
        {
            var referee = await _refereeService.GetByIdAsync(id);
            if (referee == null) return NotFound();

            return Ok(referee);
        }

        [HttpGet("resume/{resumeId}")]
        public async Task<ActionResult<IEnumerable<RefereeResponseDto>>> GetRefereesByResume(int resumeId)
        {
            var referees = await _refereeService.GetByResumeIdAsync(resumeId);
            return Ok(referees);
        }

        [HttpPost]
        public async Task<ActionResult<RefereeResponseDto>> CreateReferee(CreateRefereeDto dto)
        {
            var createdReferee = await _refereeService.CreateAsync(dto);
            if (createdReferee == null) return BadRequest("Invalid ResumeId.");

            return CreatedAtAction(nameof(GetReferee), new { id = createdReferee.Id }, createdReferee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReferee(int id, UpdateRefereeDto dto)
        {
            var updated = await _refereeService.UpdateAsync(id, dto);
            if (!updated) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReferee(int id)
        {
            var deleted = await _refereeService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}


