using Microsoft.AspNetCore.Mvc;
using ResumeApp.Server.Data;
using ResumeApp.Server.Model;
using Microsoft.EntityFrameworkCore;

namespace ResumeApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumeController : ControllerBase
    {
        private readonly ResumeDbContext _ResDb;
        public ResumeController(ResumeDbContext ResDb)
        {
            _ResDb = ResDb;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Resume>>> GetResume()
        {
            var resumes = await _ResDb.Resumes.ToListAsync();

            return Ok(resumes);
        }
    }
}
