using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RaceDayMVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;

            // CHANGED TO HTTP TO BYPASS SSL ISSUES
            _httpClient.BaseAddress = new Uri("http://localhost:5115/");
        }

        private void AddAuthCookies()
        {
            var cookies = _httpContextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
            if (!string.IsNullOrEmpty(cookies))
            {
                _httpClient.DefaultRequestHeaders.Remove("Cookie");
                _httpClient.DefaultRequestHeaders.Add("Cookie", cookies);
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            AddAuthCookies();
            return await _httpClient.GetAsync(endpoint);
        }

        public async Task<HttpResponseMessage> PostAsync(string endpoint, object data)
        {
            AddAuthCookies();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(endpoint, content);
        }

        public async Task<HttpResponseMessage> PutAsync(string endpoint, object data)
        {
            AddAuthCookies();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PutAsync(endpoint, content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            AddAuthCookies();
            return await _httpClient.DeleteAsync(endpoint);
        }

        public async Task<HttpResponseMessage> UploadFileAsync(string endpoint, IFormFile file)
        {
            AddAuthCookies();
            var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "file", file.FileName);
            return await _httpClient.PostAsync(endpoint, content);
        }
    }
}