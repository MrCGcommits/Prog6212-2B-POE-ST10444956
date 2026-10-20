namespace RaceDayAPI.Models
{
    public class Result
    {
        public int ResultId { get; set; }
        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = null!;
        public TimeSpan FinishTime { get; set; }
        public int OverallPosition { get; set; }
    }
}