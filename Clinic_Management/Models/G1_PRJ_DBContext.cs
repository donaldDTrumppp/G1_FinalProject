using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Clinic_Management.Models
{
    public partial class G1_PRJ_DBContext : DbContext
    {
        public G1_PRJ_DBContext()
        {
        }

        public G1_PRJ_DBContext(DbContextOptions<G1_PRJ_DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Appointment> Appointments { get; set; } = null!;
        public virtual DbSet<AppointmentStatus> AppointmentStatuses { get; set; } = null!;
        public virtual DbSet<Branch> Branches { get; set; } = null!;
        public virtual DbSet<MedicalRecord> MedicalRecords { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Patient> Patients { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Specialist> Specialists { get; set; } = null!;
        public virtual DbSet<Staff> Staff { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserStatus> UserStatuses { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("MyCnn"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("Appointment");

                entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");

                entity.Property(e => e.BranchId).HasColumnName("branch_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.DoctorId).HasColumnName("doctor_id");

                entity.Property(e => e.PatientAddress)
                    .HasMaxLength(100)
                    .HasColumnName("patient_address");

                entity.Property(e => e.PatientDob)
                    .HasColumnType("date")
                    .HasColumnName("patient_dob");

                entity.Property(e => e.PatientEmail)
                    .HasMaxLength(50)
                    .HasColumnName("patient_email");

                entity.Property(e => e.PatientId).HasColumnName("patient_id");

                entity.Property(e => e.PatientName)
                    .HasMaxLength(50)
                    .HasColumnName("patient_name");

                entity.Property(e => e.PatientPhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("patient_phone_number");

                entity.Property(e => e.RealTimeShowUp)
                    .HasColumnType("datetime")
                    .HasColumnName("real_time_show_up");

                entity.Property(e => e.ReceptionistId).HasColumnName("receptionist_id");

                entity.Property(e => e.RequestedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("requested_time");

                entity.Property(e => e.Specialist).HasColumnName("specialist");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Appointme__branc__4BAC3F29");

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.AppointmentDoctors)
                    .HasForeignKey(d => d.DoctorId)
                    .HasConstraintName("FK__Appointme__docto__4CA06362");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK__Appointme__patie__4D94879B");

                entity.HasOne(d => d.Receptionist)
                    .WithMany(p => p.AppointmentReceptionists)
                    .HasForeignKey(d => d.ReceptionistId)
                    .HasConstraintName("FK__Appointme__recep__4E88ABD4");

                entity.HasOne(d => d.SpecialistNavigation)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.Specialist)
                    .HasConstraintName("FK__Appointme__speci__4F7CD00D");

                entity.HasOne(d => d.StatusNavigation)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.Status)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Appointme__statu__5070F446");
            });

            modelBuilder.Entity<AppointmentStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("PK__Appointm__3683B5312675EEF1");

                entity.ToTable("Appointment_Status");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.StatusName)
                    .HasMaxLength(100)
                    .HasColumnName("status_name");
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.ToTable("Branch");

                entity.Property(e => e.BranchId).HasColumnName("branch_id");

                entity.Property(e => e.BranchName)
                    .HasMaxLength(100)
                    .HasColumnName("branch_name");
            });

            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.ToTable("Medical_Record");

                entity.HasIndex(e => e.AppointmentId, "UQ__Medical___A50828FD2527D518")
                    .IsUnique();

                entity.Property(e => e.MedicalrecordId).HasColumnName("medicalrecord_id");

                entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.Diagnosis).HasColumnName("diagnosis");

                entity.Property(e => e.DoctorId).HasColumnName("doctor_id");

                entity.Property(e => e.PatientId).HasColumnName("patient_id");

                entity.Property(e => e.Symptoms).HasColumnName("symptoms");

                entity.Property(e => e.Treatment).HasColumnName("treatment");

                entity.Property(e => e.VisitTime)
                    .HasColumnType("datetime")
                    .HasColumnName("visit_time");

                entity.HasOne(d => d.Appointment)
                    .WithOne(p => p.MedicalRecord)
                    .HasForeignKey<MedicalRecord>(d => d.AppointmentId)
                    .HasConstraintName("FK__Medical_R__appoi__5165187F");

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.MedicalRecords)
                    .HasForeignKey(d => d.DoctorId)
                    .HasConstraintName("FK__Medical_R__docto__52593CB8");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.MedicalRecords)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK__Medical_R__patie__534D60F1");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.NotificationId).HasColumnName("notification_id");

                entity.Property(e => e.Content)
                    .HasMaxLength(100)
                    .HasColumnName("content");

                entity.Property(e => e.Datetime)
                    .HasColumnType("datetime")
                    .HasColumnName("datetime");

                entity.Property(e => e.Link)
                    .HasMaxLength(100)
                    .HasColumnName("link");

                entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.ReceiverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Notificat__recei__5441852A");
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("Patient");

                entity.Property(e => e.PatientId)
                    .ValueGeneratedNever()
                    .HasColumnName("patient_id");

                entity.Property(e => e.HealthInsurance)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("health_insurance");

                entity.Property(e => e.NumberOfVisits).HasColumnName("number_of_visits");

                entity.HasOne(d => d.PatientNavigation)
                    .WithOne(p => p.Patient)
                    .HasForeignKey<Patient>(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Patient__patient__5535A963");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(100)
                    .HasColumnName("role_name");
            });

            modelBuilder.Entity<Specialist>(entity =>
            {
                entity.ToTable("Specialist");

                entity.Property(e => e.SpecialistId).HasColumnName("specialist_id");

                entity.Property(e => e.SpecialistName)
                    .HasMaxLength(100)
                    .HasColumnName("specialist_name");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__Staff__B9BE370FB3DDDCFD");

                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .HasColumnName("user_id");

                entity.Property(e => e.Cccd)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("CCCD");

                entity.Property(e => e.DoctorDepartmentId).HasColumnName("doctor_department_id");

                entity.Property(e => e.DoctorSpecialist).HasColumnName("doctor_specialist");

                entity.Property(e => e.HireDate).HasColumnType("date");

                entity.Property(e => e.Image)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.DoctorDepartment)
                    .WithMany(p => p.Staff)
                    .HasForeignKey(d => d.DoctorDepartmentId)
                    .HasConstraintName("FK__Staff__doctor_de__5629CD9C");

                entity.HasOne(d => d.DoctorSpecialistNavigation)
                    .WithMany(p => p.Staff)
                    .HasForeignKey(d => d.DoctorSpecialist)
                    .HasConstraintName("FK__Staff__doctor_sp__571DF1D5");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Staff)
                    .HasForeignKey<Staff>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Staff__user_id__5812160E");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.Address)
                    .HasMaxLength(100)
                    .HasColumnName("address");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("dob");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("phone_number");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("username");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__User__role_id__59063A47");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__User__status_id__59FA5E80");
            });

            modelBuilder.Entity<UserStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("PK__UserStat__3683B5318135096E");

                entity.ToTable("UserStatus");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.StatusName)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("status_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
