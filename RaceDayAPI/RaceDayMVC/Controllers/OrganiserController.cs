using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;
using System.Text.Json;

namespace RaceDayMVC.Controllers
{
    [Authorize(Roles = "Organiser")]
    public class OrganiserController : Controller
    {
        private readonly ApiService _apiService;

        public OrganiserController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // 1. Dashboard showing all events the Organiser created
        public async Task<IActionResult> Dashboard()
        {
            var response = await _apiService.GetAsync("api/events");

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<EventViewModel>());
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var events = JsonSerializer.Deserialize<List<EventViewModel>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(events);
        }

        // 2. GET: Create Event Page
        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View();
        }

        // 3. POST: Create Event (with Azure Blob Image Upload)
        [HttpPost]
        public async Task<IActionResult> CreateEvent(string eventName, string description, DateTime eventDate, string location, decimal distance, string eventType, IFormFile bannerImage)
        {
            string? imageUrl = null;

            // Upload Image to API -> Azure Blob
            if (bannerImage != null && bannerImage.Length > 0)
            {
                var uploadResponse = await _apiService.UploadFileAsync("api/upload/image", bannerImage);

                if (uploadResponse.IsSuccessStatusCode)
                {
                    var uploadResult = await uploadResponse.Content.ReadFromJsonAsync<JsonElement>();
                    imageUrl = uploadResult.GetProperty("imageUrl").GetString();
                }
                else
                {
                    var uploadError = await uploadResponse.Content.ReadAsStringAsync();
                    ViewBag.Error = $"Image Upload Failed. Status: {uploadResponse.StatusCode}. Details: {uploadError}";
                    return View();
                }
            }

            // Send Event Data to API
            var eventData = new
            {
                eventName,
                description,
                eventDate,
                location,
                distance,
                eventType,
                bannerImageUrl = imageUrl
            };

            var response = await _apiService.PostAsync("api/events", eventData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Dashboard");
            }

            var errorDetails = await response.Content.ReadAsStringAsync();
            ViewBag.Error = $"Failed to create event. Status: {response.StatusCode}. Details: {errorDetails}";

            return View();
        }

        // 4. GET: Manage Categories Page
        [HttpGet]
        public async Task<IActionResult> ManageCategories(int eventId)
        {
            // Get event details
            var eventResponse = await _apiService.GetAsync($"api/events/{eventId}");
            if (!eventResponse.IsSuccessStatusCode) return NotFound();

            var eventJson = await eventResponse.Content.ReadAsStringAsync();
            var eventItem = JsonSerializer.Deserialize<EventViewModel>(eventJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Get existing categories
            var catResponse = await _apiService.GetAsync($"api/categories/event/{eventId}");
            if (catResponse.IsSuccessStatusCode)
            {
                var catJson = await catResponse.Content.ReadAsStringAsync();
                eventItem.Categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(catJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            ViewBag.EventId = eventId;
            ViewBag.EventName = eventItem.EventName;
            return View(eventItem);
        }

        // 5. POST: Add Category
        [HttpPost]
        public async Task<IActionResult> AddCategory(int eventId, string categoryName, int maxParticipants)
        {
            var response = await _apiService.PostAsync("api/categories", new { eventId, categoryName, maxParticipants });

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ManageCategories", new { eventId = eventId });
            }

            // If the API fails, capture the error and show it on the view
            var errorDetails = await response.Content.ReadAsStringAsync();
            TempData["Error"] = $"Failed to add category. Status: {response.StatusCode}. Details: {errorDetails}";

            return RedirectToAction("ManageCategories", new { eventId = eventId });
        }
    }
}