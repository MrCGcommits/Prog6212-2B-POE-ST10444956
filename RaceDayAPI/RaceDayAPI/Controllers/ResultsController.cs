using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using System.Security.Claims;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        public class ResultDto
        {
            public int EnrollmentId { get; set; }
            public TimeSpan FinishTime { get; set; }
            public int OverallPosition { get; set; }
        }

        // POST: api/results (Organiser ONLY)
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> AddResult([FromBody] ResultDto request)
        {
            var enrollment = await _context.Enrollments.FindAsync(request.EnrollmentId);
            if (enrollment == null) return NotFound("Enrollment not found.");

            var result = new Result
            {
                EnrollmentId = request.EnrollmentId,
                FinishTime = request.FinishTime,
                OverallPosition = request.OverallPosition
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return Ok(result);
        }

        // GET: api/results/my-results (Participant ONLY)
        [HttpGet("my-results")]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<IEnumerable<Result>>> GetMyResults()
        {
            var participantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (participantId == null) return Unauthorized();

            var results = await _context.Results
                .Include(r => r.Enrollment)
                    .ThenInclude(e => e.Event)
                .Include(r => r.Enrollment)
                    .ThenInclude(e => e.Category)
                .Where(r => r.Enrollment.ParticipantId == int.Parse(participantId))
                .ToListAsync();

            return Ok(results);
        }
    }
}