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

        // DTO for capturing results
        public class ResultDto
        {
            public int EnrollmentId { get; set; }
            public string FinishTime { get; set; } = string.Empty;
            public int OverallPosition { get; set; }
        }

        // NEW: DTO for reading results safely (fixes the 500 error)
        public class ResultReadDto
        {
            public int ResultId { get; set; }
            public string EventName { get; set; } = string.Empty;
            public string CategoryName { get; set; } = string.Empty;
            public string FinishTime { get; set; } = string.Empty;
            public int OverallPosition { get; set; }
        }

        // POST: api/results (Organiser ONLY)
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CaptureResult(ResultDto request)
        {
            var organiserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (organiserId == null) return Unauthorized();

            var enrollment = await _context.Enrollments
                .Include(e => e.Event)
                .FirstOrDefaultAsync(e => e.EnrollmentId == request.EnrollmentId);

            if (enrollment == null) return NotFound(new { message = "Enrollment not found." });

            if (enrollment.Event!.OrganiserId != int.Parse(organiserId))
            {
                return Forbid();
            }

            var existingResult = await _context.Results.FirstOrDefaultAsync(r => r.EnrollmentId == request.EnrollmentId);
            if (existingResult != null)
            {
                return Conflict(new { message = "A result has already been captured for this enrollment." });
            }

            if (!TimeSpan.TryParse(request.FinishTime, out TimeSpan parsedTime))
            {
                return BadRequest(new { message = "Invalid finish time format. Use HH:MM:SS." });
            }

            var newResult = new Result
            {
                EnrollmentId = request.EnrollmentId,
                FinishTime = parsedTime,
                OverallPosition = request.OverallPosition
            };

            _context.Results.Add(newResult);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Result captured successfully.", resultId = newResult.ResultId });
        }

        // GET: api/results/my (Participant ONLY)
        [HttpGet("my")]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<IEnumerable<ResultReadDto>>> GetMyResults()
        {
            var participantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (participantId == null) return Unauthorized();

            // Select only the specific fields we need into the DTO
            var results = await _context.Results
                .Include(r => r.Enrollment)
                    .ThenInclude(e => e.Event)
                .Include(r => r.Enrollment)
                    .ThenInclude(e => e.Category)
                .Where(r => r.Enrollment!.ParticipantId == int.Parse(participantId))
                .Select(r => new ResultReadDto
                {
                    ResultId = r.ResultId,
                    EventName = r.Enrollment!.Event!.EventName,
                    CategoryName = r.Enrollment.Category!.CategoryName,
                    FinishTime = r.FinishTime.ToString(@"hh\:mm\:ss"),
                    OverallPosition = r.OverallPosition
                })
                .ToListAsync();

            return Ok(results);
        }
    }
}