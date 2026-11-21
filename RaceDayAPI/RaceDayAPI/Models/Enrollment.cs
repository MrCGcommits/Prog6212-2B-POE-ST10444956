namespace RaceDayAPI.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public int ParticipantId { get; set; }
        public User Participant { get; set; } = null!;
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    }
}
