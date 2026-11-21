using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Services;
using System.Security.Claims;
using System.Text.Json;

namespace RaceDayMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // --- LOGIN ---
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var response = await _apiService.PostAsync("api/auth/login", new { email, password });

            if (response.IsSuccessStatusCode)
            {
                // Read the JSON response safely
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonDocument.Parse(jsonString).RootElement;

                string? role = result.TryGetProperty("role", out var roleProp) ? roleProp.GetString() : null;

                if (string.IsNullOrEmpty(role))
                {
                    ViewBag.Error = "Login succeeded, but the API did not return a valid role.";
                    return View();
                }

                // Create the MVC authentication cookie
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Sign in to the MVC app
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                // Forward the API's cookie to the browser
                if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
                {
                    foreach (var cookie in cookies) Response.Headers.Append("Set-Cookie", cookie);
                }

                // Redirect based on role
                if (role == "Organiser")
                {
                    return RedirectToAction("Dashboard", "Organiser");
                }
                else
                {
                    return RedirectToAction("Index", "Participant");
                }
            }

            var errorDetails = await response.Content.ReadAsStringAsync();
            ViewBag.Error = $"Login Failed. Status: {response.StatusCode}. Details: {errorDetails}";
            return View();
        }

        // --- REGISTER ---
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string firstName, string lastName, string role)
        {
            var response = await _apiService.PostAsync("api/auth/register", new { email, password, firstName, lastName, role });

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }

            var errorDetails = await response.Content.ReadAsStringAsync();
            ViewBag.Error = $"Registration failed: {errorDetails}";
            return View();
        }

        // --- LOGOUT ---
        public async Task<IActionResult> Logout()
        {
            // Tell API to logout
            await _apiService.PostAsync("api/auth/logout", new { });

            // Tell MVC to logout
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();
    }
}