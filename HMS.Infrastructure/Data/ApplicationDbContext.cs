using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace HMS.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalHistory> MedicalHistories { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<PatientVisit> PatientVisits { get; set; }

        public DbSet<Doctor> Doctors { get; set; }



        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;
        //public DbSet<Doctor> Doctors { get; set; }
        //public DbSet<Appointment> Appointments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserRole composite key
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // User -> UserRoles relationship
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            // Role -> UserRoles relationship
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Optional: Make Email and Username unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<MedicalHistory>()
            .HasOne(m => m.Patient)
            .WithMany(p => p.MedicalHistories)
            .HasForeignKey(m => m.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Allergy>()
            .HasKey(a => a.Id);

            modelBuilder.Entity<Allergy>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Allergies) // Make sure Patient has ICollection<Allergy> Allergies
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Insurance>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<Insurance>()
             .HasOne(i => i.Patient)
             .WithMany(p => p.Insurances)
             .HasForeignKey(i => i.PatientId)
             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PatientVisit>()
            .HasKey(i => i.Id);

            modelBuilder.Entity<PatientVisit>()
                .HasOne(i => i.Patient)
                .WithMany(p => p.PatientVisits)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(d => d.Id);
            });

        }
    }
}
