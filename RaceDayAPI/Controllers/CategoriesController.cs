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

        // DTO for Creating/Updating Categories
        public class CategoryDto
        {
            public string CategoryName { get; set; } = string.Empty;
            public int MaxParticipants { get; set; }
        }

        // GET: api/categories/event/{eventId} (Public)
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesForEvent(int eventId)
        {
            var eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId);
            if (!eventExists)
            {
                return NotFound(new { message = "Event not found." });
            }

            return await _context.Categories.Where(c => c.EventId == eventId).ToListAsync();
        }

        // POST: api/categories/event/{eventId} (Organiser ONLY)
        [HttpPost("event/{eventId}")]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<Category>> CreateCategory(int eventId, CategoryDto request)
        {
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            var newCategory = new Category
            {
                CategoryName = request.CategoryName,
                MaxParticipants = request.MaxParticipants,
                EventId = eventId
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category created successfully.", categoryId = newCategory.CategoryId });
        }

        // DELETE: api/categories/{id} (Organiser ONLY)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category deleted successfully." });
        }
    }
}