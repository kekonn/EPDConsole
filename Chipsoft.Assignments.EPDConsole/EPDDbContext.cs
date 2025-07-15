using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chipsoft.Assignments.EPDConsole.Models;

namespace Chipsoft.Assignments.EPDConsole
{
    public class EPDDbContext : DbContext
    {
        public const int AddressLength = 255;
        public const int NameLength = 125;
        private const int TelephoneLength = 50;

        // The following configures EF to create a Sqlite database file in the
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source=epd.db");
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var patientModel = modelBuilder.Entity<Patient>();
            patientModel.HasKey(p => p.INSZ); // TODO: Set Length
            patientModel.Property(p => p.Address).HasMaxLength(AddressLength);
            patientModel.Property(p => p.TelephoneNumber).HasMaxLength(TelephoneLength);
            patientModel.Property(p => p.Name).HasMaxLength(NameLength);
            
            var doctorModel = modelBuilder.Entity<Doctor>();
            doctorModel.HasKey(d => d.INSZ);
            doctorModel.Property(d => d.Name).HasMaxLength(NameLength);
            
            var appointmentModel = modelBuilder.Entity<Appointment>();
            appointmentModel.HasKey(a => a.Id);
            appointmentModel.HasOne(a => a.Patient).WithMany(p => p.Appointments);
            appointmentModel.HasOne(a => a.Doctor).WithMany(d => d.Appointments);
        }
    }
}
