namespace RaceDayMVC.Models;

public class RaceEnrollment
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; }
    public DateTime EnrollmentDate { get; set; }

    public RaceEvent? Event { get; set; }
}