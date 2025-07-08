namespace Chipsoft.Assignments.EPDConsole.Models;

public class Appointment
{
    public Guid Id { get; set; }
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}