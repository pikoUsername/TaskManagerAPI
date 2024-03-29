using System.ComponentModel.DataAnnotations;

namespace TaskManager.Database.Models; 

public class WorkVisit
{
    [Key]
    [Required] public Guid Id { get; set; }
    public DateTime VisitedAt { get; set; }
    public DateTime EndedAt { get; set; }

    // Many-to-one relationships: WorkVisit -> User, DayTimetable
    //public UserModel User { get; set; } 
    [Required]
    public DayTimetable DayTimetable { get; set; }    
}