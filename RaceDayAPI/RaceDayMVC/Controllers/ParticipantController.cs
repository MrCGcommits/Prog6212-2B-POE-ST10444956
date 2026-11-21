using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;
using System.Text.Json;

namespace RaceDayMVC.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly ApiService _apiService;

        public ParticipantController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // 1. Browse Events (Public - Anyone can view)
        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString)
        {
            var response = await _apiService.GetAsync("api/events");

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<EventViewModel>());
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var events = JsonSerializer.Deserialize<List<EventViewModel>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Simple search filtering
            if (!string.IsNullOrEmpty(searchString))
            {
                events = events.Where(e => e.EventName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                           e.Location.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.CurrentFilter = searchString;
            return View(events);
        }

        // 2. Event Details (Public)
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _apiService.GetAsync($"api/events/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var jsonString = await response.Content.ReadAsStringAsync();
            var eventItem = JsonSerializer.Deserialize<EventViewModel>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(eventItem);
        }

        // 3. Enrol in Event (Participant ONLY)
        [HttpPost]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> Enroll(int eventId, int categoryId)
        {
            var response = await _apiService.PostAsync("api/enrollments", new { eventId, categoryId });

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Successfully enrolled!";
                return RedirectToAction("MyEvents");
            }

            // Handle error (e.g., already enrolled)
            var errorDetails = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Failed to enrol. Details: {errorDetails}";

            return RedirectToAction("Details", new { id = eventId });
        }

        // 4. My Enrolments (Participant ONLY)
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> MyEvents()
        {
            var response = await _apiService.GetAsync("api/enrollments/my");
            if (!response.IsSuccessStatusCode) return View(new List<JsonElement>());

            var jsonString = await response.Content.ReadAsStringAsync();
            var enrollments = JsonSerializer.Deserialize<List<JsonElement>>(jsonString);

            return View(enrollments);
        }

        // 5. My Results (Participant ONLY)
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> MyResults()
        {
            var response = await _apiService.GetAsync("api/results/my-results");
            if (!response.IsSuccessStatusCode) return View(new List<JsonElement>());

            var jsonString = await response.Content.ReadAsStringAsync();
            var results = JsonSerializer.Deserialize<List<JsonElement>>(jsonString);

            return View(results);
        }
    }
}