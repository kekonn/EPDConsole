namespace Chipsoft.Assignments.EPDConsole.Models;

public class Doctor
{
    public string INSZ { get; set; }
    public string Name { get; set; }
    public IList<Appointment> Appointments { get; set; }
}