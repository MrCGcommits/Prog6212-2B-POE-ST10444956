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
    public class EventsController : ControllerBase
    {
        // DTO for Creating/Updating Events
        public class EventDto
        {
            public string EventName { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public DateTime EventDate { get; set; }
            public string Location { get; set; } = string.Empty;
            public decimal Distance { get; set; }
            public string EventType { get; set; } = string.Empty;
        }

        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events (Public - Both roles and guests can view)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            return await _context.Events.Include(e => e.Categories).ToListAsync();
        }

        // GET: api/events/{id} (Public - Both roles and guests can view)
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            return eventItem;
        }

        // POST: api/events (Organiser ONLY)
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<Event>> CreateEvent(EventDto request) // <-- CHANGED TO EventDto
        {
            var organiserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (organiserId == null)
            {
                return Unauthorized();
            }

            var newEvent = new Event
            {
                EventName = request.EventName,
                Description = request.Description,
                EventDate = request.EventDate,
                Location = request.Location,
                Distance = request.Distance,
                EventType = request.EventType,
                OrganiserId = int.Parse(organiserId)
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.EventId }, newEvent);
        }

        // PUT: api/events/{id} (Organiser ONLY)
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateEvent(int id, EventDto request) // <-- CHANGED TO EventDto
        {
            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            // Ensure the Organiser can only edit THEIR OWN events
            var organiserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existingEvent.OrganiserId.ToString() != organiserId)
            {
                return Forbid(); // 403 Forbidden
            }

            // Update fields
            existingEvent.EventName = request.EventName;
            existingEvent.Description = request.Description;
            existingEvent.EventDate = request.EventDate;
            existingEvent.Location = request.Location;
            existingEvent.Distance = request.Distance;
            existingEvent.EventType = request.EventType;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Event updated successfully." });
        }

        // DELETE: api/events/{id} (Organiser ONLY)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            // Ensure the Organiser can only delete THEIR OWN events
            var organiserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (eventItem.OrganiserId.ToString() != organiserId)
            {
                return Forbid(); // 403 Forbidden
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Event deleted successfully." });
        }
    }
}