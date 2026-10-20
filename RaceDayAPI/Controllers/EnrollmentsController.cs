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
    public class EnrollmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrollmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // DTO for creating an enrollment
        public class EnrollmentDto
        {
            public int EventId { get; set; }
            public int CategoryId { get; set; }
        }

        // POST: api/enrollments (Participant ONLY)
        [HttpPost]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> CreateEnrollment(EnrollmentDto request)
        {
            var participantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (participantId == null) return Unauthorized();

            // Check if event exists
            var eventItem = await _context.Events.FindAsync(request.EventId);
            if (eventItem == null) return NotFound(new { message = "Event not found." });

            // Check if category exists and belongs to the event
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId && c.EventId == request.EventId);
            if (category == null) return BadRequest(new { message = "Invalid category for this event." });

            // Check if participant is already enrolled in this event
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.ParticipantId == int.Parse(participantId) && e.EventId == request.EventId);

            if (existingEnrollment != null)
            {
                return Conflict(new { message = "You are already enrolled in this event." });
            }

            var newEnrollment = new Enrollment
            {
                ParticipantId = int.Parse(participantId),
                EventId = request.EventId,
                CategoryId = request.CategoryId,
                EnrollmentDate = DateTime.UtcNow
            };

            _context.Enrollments.Add(newEnrollment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully enrolled in the event.", enrollmentId = newEnrollment.EnrollmentId });
        }

        // GET: api/enrollments/my (Participant ONLY)
        [HttpGet("my")]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetMyEnrollments()
        {
            var participantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (participantId == null) return Unauthorized();

            var enrollments = await _context.Enrollments
                .Include(e => e.Event)
                .Include(e => e.Category)
                .Where(e => e.ParticipantId == int.Parse(participantId))
                .ToListAsync();

            return Ok(enrollments);
        }

        // GET: api/enrollments/event/{eventId} (Organiser ONLY)
        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollmentsForEvent(int eventId)
        {
            var organiserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (organiserId == null) return Unauthorized();

            // Ensure the organiser owns this event
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null) return NotFound(new { message = "Event not found." });
            if (eventItem.OrganiserId != int.Parse(organiserId)) return Forbid();

            var enrollments = await _context.Enrollments
                .Include(e => e.Participant)
                .Include(e => e.Category)
                .Where(e => e.EventId == eventId)
                .ToListAsync();

            return Ok(enrollments);
        }
    }
}
