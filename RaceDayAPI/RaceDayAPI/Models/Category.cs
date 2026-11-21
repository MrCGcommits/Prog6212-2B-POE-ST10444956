using System.Text.Json.Serialization;

namespace RaceDayAPI.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public int EventId { get; set; }

        [JsonIgnore] // <-- THIS FIXES THE VALIDATION ERROR
        public Event Event { get; set; } = null!;

        public string CategoryName { get; set; } = string.Empty; // e.g., "Under 20", "Senior", "10km"
        public int MaxParticipants { get; set; }
    }
}