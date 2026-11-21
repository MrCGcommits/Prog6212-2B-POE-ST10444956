using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // DTO for creating a category (avoids validation errors on the Event object)
        public class CategoryDto
        {
            public int EventId { get; set; }
            public string CategoryName { get; set; } = string.Empty;
            public int MaxParticipants { get; set; }
        }

        // GET: api/categories/event/{eventId}
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesForEvent(int eventId)
        {
            return await _context.Categories.Where(c => c.EventId == eventId).ToListAsync();
        }

        // POST: api/categories (Organiser ONLY)
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] CategoryDto request)
        {
            // Verify the event exists
            var eventItem = await _context.Events.FindAsync(request.EventId);
            if (eventItem == null) return NotFound("Event not found.");

            // Map DTO to Entity
            var newCategory = new Category
            {
                EventId = request.EventId,
                CategoryName = request.CategoryName,
                MaxParticipants = request.MaxParticipants
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return Ok(newCategory);
        }
    }
}