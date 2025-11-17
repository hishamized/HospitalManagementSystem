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
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Slot> Slots { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<DoctorDocument> DoctorDocuments { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<DoctorWard> DoctorWards { get; set; }

        public DbSet<Feedback> Feedback { get; set; }

        public DbSet<UserOtp> UserOtp { get; set; }

        public DbSet<Bill> Bill { get; set; }

        public DbSet<Bed> Bed { get; set; }

        public DbSet<PatientBedWard> PatientBedWards { get; set; }



        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;

        public DbSet<DoctorRound> DoctorRounds { get; set; }

        //public DbSet<Doctor> Doctors { get; set; }
        //public DbSet<Appointment> Appointments { get; set; }

        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatRoomUser> ChatRoomUsers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageStatus> MessageStatuses { get; set; }

        public DbSet<Payment> Payments { get; set; }


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
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.PatientCode)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(p => p.FullName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(p => p.Email)
                      .HasMaxLength(150);

                entity.Property(p => p.ContactNumber)
                      .HasMaxLength(20);

                // ✅ Unique Constraints
                entity.HasIndex(p => p.Email)
                      .IsUnique();

                entity.HasIndex(p => p.PatientCode)
                      .IsUnique();

                entity.HasIndex(p => p.ContactNumber)
                      .IsUnique();
            });

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

                // Doctor → Department (many doctors per department)
                entity.HasOne(d => d.Department)
                      .WithMany(dep => dep.Doctors)
                      .HasForeignKey(d => d.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict); // prevents cascading deletes

                // Doctor → Slot (many doctors can share slot if logic allows)
                entity.HasOne(d => d.Slot)
                      .WithMany(s => s.Doctors)
                      .HasForeignKey(d => d.SlotId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Slot>(entity =>
            {
                entity.ToTable("Slots");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReportingTime).IsRequired();
                entity.Property(e => e.LeavingTime).IsRequired();
                entity.Property(e => e.DaysOfWeek).IsRequired();
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Departments");  // optional, explicit table name
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Name)
                      .IsRequired()
                      .HasMaxLength(100);        // optional length constraint
            });

            modelBuilder.Entity<Ward>(entity =>
            {
                entity.ToTable("Wards"); // Table name in DB

                entity.HasKey(w => w.Id);

                entity.Property(w => w.WardCode)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(w => w.WardName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(w => w.WardType)
                      .HasMaxLength(50);

                entity.Property(w => w.Location)
                      .HasMaxLength(100);

                entity.Property(w => w.Description)
                      .HasMaxLength(500);

                entity.Property(w => w.IsActive)
                      .HasDefaultValue(true);

                entity.Property(w => w.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<PatientBedWard>(entity =>
            {
                entity.ToTable("PatientBedWards");

                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Patient)
                    .WithMany() // or .WithMany(p => p.PatientBedWards) if you add a navigation in Patient
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Ward)
                    .WithMany() // you can add .WithMany(w => w.PatientBedWards) later if needed
                    .HasForeignKey(e => e.WardId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Bed)
                    .WithMany() // or .WithMany(b => b.PatientBedWards)
                    .HasForeignKey(e => e.BedId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.AssignedAt)
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                // To ensure a bed can’t be assigned to more than one active patient at a time
                entity.HasIndex(e => new { e.BedId, e.IsActive })
                    .IsUnique()
                    .HasFilter("[IsActive] = 1");

                // Optional: enforce uniqueness so one patient can’t have two active assignments
                entity.HasIndex(e => new { e.PatientId, e.IsActive })
                    .IsUnique()
                    .HasFilter("[IsActive] = 1");
            });


            modelBuilder.Entity<DoctorRound>(entity =>
            {
                entity.ToTable("DoctorRounds");

                entity.HasKey(dr => dr.Id);

                entity.Property(dr => dr.Observations).HasMaxLength(1000);
                entity.Property(dr => dr.Diagnosis).HasMaxLength(500);
                entity.Property(dr => dr.Prescriptions).HasMaxLength(1000);
                entity.Property(dr => dr.TreatmentPlan).HasMaxLength(1000);
                entity.Property(dr => dr.TestsRecommended).HasMaxLength(1000);
                entity.Property(dr => dr.FollowUpInstructions).HasMaxLength(1000);

                entity.HasOne(dr => dr.Doctor)
                      .WithMany()
                      .HasForeignKey(dr => dr.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(dr => dr.Patient)
                      .WithMany()
                      .HasForeignKey(dr => dr.PatientId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(dr => dr.Ward)
                      .WithMany()
                      .HasForeignKey(dr => dr.WardId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Apply configurations
            modelBuilder.Entity<ChatRoomUser>()
                .HasKey(cru => new { cru.ChatRoomId, cru.UserId });

            modelBuilder.Entity<MessageStatus>()
                .HasKey(ms => new { ms.MessageId, ms.UserId });

            modelBuilder.Entity<ChatRoomUser>()
                .HasOne(cru => cru.ChatRoom)
                .WithMany(cr => cr.ChatRoomUsers)
                .HasForeignKey(cru => cru.ChatRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChatRoomUser>()
                .HasOne(cru => cru.User)
                .WithMany()
                .HasForeignKey(cru => cru.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.ChatRoom)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MessageStatus>()
                .HasOne(ms => ms.Message)
                .WithMany(m => m.MessageStatuses)
                .HasForeignKey(ms => ms.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MessageStatus>()
                .HasOne(ms => ms.User)
                .WithMany()
                .HasForeignKey(ms => ms.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");

                entity.HasKey(p => p.Id);

                // Payment → Bill relationship (required foreign key)
                entity.HasOne(p => p.Bill)
                      .WithMany() // or .WithMany(b => b.Payments) if you add ICollection<Payment> to Bill
                      .HasForeignKey(p => p.BillId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Property configurations
                entity.Property(p => p.Amount)
                      .HasPrecision(18, 2); // decimal precision for currency

                entity.Property(p => p.PaymentStatus)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasDefaultValue("Unpaid");

                entity.Property(p => p.PaymentMode)
                      .HasMaxLength(50);

                entity.Property(p => p.PaymentDate)
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(p => p.CreatedAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");

                // Optional: Add index on BillId for better query performance
                entity.HasIndex(p => p.BillId);

                // Optional: Add index on PaymentStatus for filtering queries
                entity.HasIndex(p => p.PaymentStatus);
            });

        }
    }
}
