using System.ComponentModel.DataAnnotations;

namespace TaskManager.Database.Models; 

public class DayTimetable
{
    [Key]
    [Required] public Guid Id { get; set; }
    [Required]
    public DateTime StartsAt { get; set; }
    [Required]
    public DateTime EndsAt { get; set; }
    public string Type { get; set; } = DayTimeTableTypes.work;
    public DayOfWeek Day { get; set; } = DayOfWeek.Saturday;
    [Required]
    public string Name { get; set; } = string.Empty;

    // One-to-many relationship: DayTimetable -> WorkVisits
    //public ICollection<WorkVisit>? WorkVisits { get; set; }

    public static List<DayTimetable> CreateDefaultTimeTable()
    {
        List<DayTimetable> days = [];
    //    Monday,
    //Tuesday,
    //Wednesday,
    //Thursday,
    //Friday,
    //Saturday,
    //Sunday,
        days.Add(new DayTimetable
        {
            Name = "�����������",
            StartsAt = DateTime.UtcNow,  
            EndsAt = DateTime.UtcNow.AddHours(8),
            Day = DayOfWeek.Monday
        });
        days.Add(new DayTimetable
        {
            Name = "�������",
            StartsAt = DateTime.UtcNow,
            EndsAt = DateTime.UtcNow.AddHours(8),
            Day = DayOfWeek.Tuesday, 
        });
        days.Add(new DayTimetable
        {
            Name = "�����",
            StartsAt = DateTime.UtcNow,
            EndsAt = DateTime.UtcNow.AddHours(8),
            Day = DayOfWeek.Wednesday, 
        });
        days.Add(new DayTimetable
        {
            Name = "�������",
            StartsAt = DateTime.UtcNow,
            EndsAt = DateTime.UtcNow.AddHours(8),
            Day = DayOfWeek.Thursday
        });
        days.Add(new DayTimetable
        {
            Name = "�������",
            StartsAt = DateTime.UtcNow,
            EndsAt = DateTime.UtcNow.AddHours(8),
            Day = DayOfWeek.Friday
        });
        days.Add(new DayTimetable
        {
            Name = "�������",
            StartsAt = DateTime.UtcNow,
            EndsAt = DateTime.UtcNow.AddHours(8),
            Day = DayOfWeek.Saturday,
            Type = DayTimeTableTypes.weekend
        });
        days.Add(new DayTimetable
        {
            Name = "�����������",
            StartsAt = DateTime.UtcNow,
            EndsAt = DateTime.UtcNow.AddHours(8),
            Day = DayOfWeek.Saturday,
            Type = DayTimeTableTypes.weekend
        });


        return days; 
    }
}