namespace Chipsoft.Assignments.EPDConsole.Models;

public class Patient
{
    public string Name { get; set; }
    public string INSZ { get; set; }
    public string Address { get; set; }
    public string TelephoneNumber { get; set; }
    public IList<Appointment> Appointments { get; set; }

    public override string ToString()
    {
        return $"{Name} {INSZ}";
    }
}