namespace RaceDayAPI.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public int OrganiserId { get; set; }
        public User Organiser { get; set; } = null!;
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string EventType { get; set; } = string.Empty; // "run", "walk", or "cycle"

        public List<Category> Categories { get; set; } = new();
    }
}
