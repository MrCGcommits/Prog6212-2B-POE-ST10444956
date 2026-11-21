using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDayAPI.Services;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly BlobService _blobService;

        public UploadController(BlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpPost("image")]
        [Authorize(Roles = "Organiser,Participant")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            try
            {
                var url = await _blobService.UploadFileAsync(file);
                return Ok(new { imageUrl = url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}