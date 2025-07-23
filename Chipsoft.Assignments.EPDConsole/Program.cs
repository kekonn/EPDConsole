using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.CompilerServices;
using Chipsoft.Assignments.EPDConsole.Models;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using Console = Spectre.Console.AnsiConsole;


[assembly:InternalsVisibleTo("Chipsoft.Assignments.EPDConsole.Tests")]

namespace Chipsoft.Assignments.EPDConsole
{
    public class Program
    {
        private const string? DateFormat = "dd/MM/yy";

        private const string? TimeFormat = "hh:mm";

        private const int ChoicePageSize = 10;
        //Don't create EF migrations, use the reset db option
        //This deletes and recreates the db, this makes sure all tables exist

        private static void AddPatient()
        {
            Console.Write(new Rule("Voeg nieuwe patient toe"));
            // Query patient data
            var name = ConsoleUtils.ReadRequiredString("Naam patient: ");
            var insz = ConsoleUtils.ReadRequiredString("INSZ: ");
            var address = ConsoleUtils.ReadRequiredString("Adres (alles op 1 lijn, met komma's): ");
            var telephone = ConsoleUtils.ReadRequiredString("Telefoonnummer: ");
            
            var patient = new Patient
            {
                Name = name,
                Address = address,
                TelephoneNumber = telephone,
                INSZ = insz,
            };

            // Save data
            using var dbContext = new EPDDbContext();
            dbContext.Patients.Add(patient);
            dbContext.SaveChanges();
            Console.MarkupLineInterpolated($"{Emoji.Known.CheckMark} Patient {patient} werd toegevoegd.");
        }
        
        private static void DeletePatient()
        {
            using var dbContext = new EPDDbContext();

            if (!dbContext.Patients.Any()) return;
            
            // Select a patient
            var prompt = new SelectionPrompt<Patient>()
                .Title("Selecteer een patient om te verwijderen")
                .PageSize(ChoicePageSize)
                .AddChoices(dbContext.Patients.ToArray())
                .EnableSearch();
            
            var patientToDelete = Console.Prompt(prompt);

            var confirmation = Console.Confirm($"Bent u zeker dat u patient {patientToDelete} wenst te verwijderen?", false);

            if (!confirmation) return;
            
            dbContext.Patients.Remove(patientToDelete);
            dbContext.SaveChanges();
            
            Console.MarkupLineInterpolated($"{Emoji.Known.CheckMark} Patient succesvol verwijdert.");
        }

        private static void ShowAppointment()
        {
            using var dbContext = new EPDDbContext();
            
            var futureAppointments = dbContext.Appointments
                .Where(a => a.Start >= DateTime.Now)
                .OrderBy(a => a.Start)
                .ThenBy(a => a.End)
                .Include(appointment => appointment.Doctor)
                .Include(appointment => appointment.Patient)
                .ToImmutableList();

            if (!futureAppointments.Any())
            {
                Console.MarkupLine("[red]Er zijn geen geplande afspraken.[/]");
                return;
            }
            
            // construct table
            var appointmentTable = new Table();
            appointmentTable.AddColumns("Datum", "Arts", "Start", "Einde", "Patient");
            appointmentTable.Border(TableBorder.DoubleEdge);
            appointmentTable.ShowRowSeparators();
            
            // fill table
            foreach (var appointment in futureAppointments)
            {
                appointmentTable.AddRow(appointment.Start.ToString(DateFormat), // datum
                    appointment.Doctor.Name, // arts
                    appointment.Start.ToString(TimeFormat), //start
                    appointment.End.ToString(TimeFormat), // einde
                    appointment.Patient.ToString() // patient
                );
            }
            
            Console.Write(appointmentTable);

            _ = System.Console.ReadKey();
        }

        private static void AddAppointment()
        {
            using var dbContext = new EPDDbContext();

            Console.Status().Start("Afspraken laden...", ctx =>
            {
                dbContext.Appointments.Load();

                ctx.Status("Artsen laden...");
                dbContext.Doctors.Load();

                ctx.Status("Patienten laden...");
                dbContext.Patients.Load();
            });
            
            Console.MarkupLine("[green]" + Emoji.Known.CheckMark + " Klaar![/]");
            Console.Write(new Rule("Afspraak maken"));
            
            Console.Write(new Rule("Stap 1: Patient zoeken"));
            var patientPrompt = new SelectionPrompt<Patient>()
                .Title("Selecteer de patient")
                .EnableSearch()
                .PageSize(ChoicePageSize)
                .AddChoices(dbContext.Patients);

            var selectedPatient = Console.Prompt(patientPrompt);
            
            Console.Write(new Rule("Stap 2: Arts zoeken"));
            var doctorPrompt = new SelectionPrompt<Doctor>()
                .Title("Kies een arts")
                .EnableSearch()
                .PageSize(ChoicePageSize)
                .AddChoices(dbContext.Doctors)
                .UseConverter(d => d.Name);
            
            var selectedDoctor = Console.Prompt(doctorPrompt);
            
            Console.Write(new Rule("Stap 3: Datum en tijd kiezen"));
            var aptDate = Console.Prompt(new SelectionPrompt<DateTime>()
                .Title("Kies een dag")
                .EnableSearch()
                .AddChoices(DateTimeUtils.GetMidnightDates(DateTime.Now, 14))
                .PageSize(7)
                .UseConverter(d => d.ToString("ddd dd-MM"))
            );

            var startDate = DateTimeUtils.RoundToNextInterval(DateTime.Now, TimeSpan.FromMinutes(15));
            var startChoices = DateTimeUtils.GetIncrementedDates(startDate, 40, TimeSpan.FromMinutes(15))
                .Where(d => d.Date == startDate.Date);

            var aptStart = Console.Prompt(new SelectionPrompt<DateTime>()
                .Title("Kies een starttijd")
                .PageSize(8)
                .AddChoices(startChoices)
                .UseConverter(d => d.ToString(TimeFormat))
            );

            var appointment = new Appointment
            {
                Doctor = selectedDoctor,
                Patient = selectedPatient,
                Start = aptStart,
                End = aptStart.AddMinutes(15)
            };

            var confirmationPanel = new Panel(new Rows(
                new Text($"Arts: {appointment.Doctor.Name}"),
                new Text($"Patient: {appointment.Patient}")
            )).Border(BoxBorder.Rounded)
                .Header($"Afspraak op {appointment.Start.ToLongDateString()}");
            
            Console.Write(confirmationPanel);

            if (!Console.Confirm("Bevestig afspraak?")) return;
            
            dbContext.Appointments.Add(appointment);
            dbContext.SaveChanges();
            
            Console.MarkupLine("[green]"+ Emoji.Known.CheckMark + " Afspraak opgeslagen.[/]");
        }

        private static void DeletePhysician()
        {
            using var dbContext = new EPDDbContext();
            dbContext.Doctors.Load();

            if (!dbContext.Doctors.Any())
            {
                Console.MarkupLine("[red]Er zijn geen artsen gekend.[/]");
                return;
            }

            var prompt = new SelectionPrompt<Doctor>()
                .Title("Selecteer een arts om te verwijderen")
                .PageSize(ChoicePageSize)
                .AddChoices(dbContext.Doctors.ToList())
                .EnableSearch();
            
            var doctorToDelete = Console.Prompt(prompt);
            
            var confirmation = Console.Confirm($"Bent u zeker dat u arts {doctorToDelete.Name} wilt verwijderen?", false);
            if (!confirmation) return; // cancel deletion
            
            dbContext.Doctors.Remove(doctorToDelete);
            dbContext.SaveChanges();
            
            Console.MarkupLineInterpolated($"{Emoji.Known.CheckMark} Arts {doctorToDelete.Name} werd verwijderd.");
        }

        private static void AddPhysician()
        {
            Console.Write(new Rule("Voeg een nieuwe arts toe"));
            var name = ConsoleUtils.ReadRequiredString("Naam arts: ");
            var insz = ConsoleUtils.ReadRequiredString("INSZ: ");

            var doctor = new Doctor
            {
                INSZ = insz,
                Name = name,
            };
            
            using var dbContext = new EPDDbContext();
            dbContext.Doctors.Add(doctor);
            dbContext.SaveChanges();
            
            Console.MarkupLineInterpolated($"{Emoji.Known.CheckMark} Arts {doctor.Name} werd opgeslagen.");
        }
        
        #region FreeCodeForAssignment
        static void Main(string[] args)
        {
            Console.Clear();
            var belgianCulture = CultureInfo.GetCultureInfo("nl-BE");
            Thread.CurrentThread.CurrentCulture = belgianCulture;
            Thread.CurrentThread.CurrentUICulture = belgianCulture;
            while (ShowMenu())
            {
                //Continue
            }
        }

        public static bool ShowMenu()
        {
            Console.WriteLine(); // empty line
            foreach (var line in File.ReadAllLines("logo.txt"))
            {
                Console.WriteLine(line);
            }
            Console.Write(new Rule());
            Console.WriteLine("1 - Patient toevoegen");
            Console.WriteLine("2 - Patienten verwijderen");
            Console.WriteLine("3 - Arts toevoegen");
            Console.WriteLine("4 - Arts verwijderen");
            Console.WriteLine("5 - Afspraak toevoegen");
            Console.WriteLine("6 - Afspraken inzien");
            Console.WriteLine("7 - Sluiten");
            Console.WriteLine("8 - Reset db");

            if (int.TryParse(System.Console.ReadLine(), out int option))
            {
                Console.WriteLine();
                switch (option)
                {
                    case 1:
                        AddPatient();
                        return true;
                    case 2:
                        DeletePatient();
                        return true;
                    case 3:
                        AddPhysician();
                        return true;
                    case 4:
                        DeletePhysician();
                        return true;
                    case 5:
                        AddAppointment();
                        return true;
                    case 6:
                        ShowAppointment();
                        return true;
                    case 7:
                        return false;
                    case 8:
                        EPDDbContext dbContext = new EPDDbContext();
                        dbContext.Database.EnsureDeleted();
                        dbContext.Database.EnsureCreated();
                        return true;
                    default:
                        return true;
                }
            }
            return true;
        }

        #endregion
    }
}