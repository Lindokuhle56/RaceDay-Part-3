using System.ComponentModel.DataAnnotations;

namespace RaceDayMVC.Models;

public class RaceEvent
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string Description { get; set; } = "";

    public DateTime EventDate { get; set; }

    [Required]
    public string Location { get; set; } = "";

    public decimal Distance { get; set; }

    public string EventType { get; set; } = "";

    public decimal EntryFee { get; set; }
}