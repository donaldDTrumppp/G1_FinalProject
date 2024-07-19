using System;
using System.Collections.Generic;

namespace Clinic_Management.Models
{
    public partial class User
    {
        public User()
        {
            AppointmentDoctors = new HashSet<Appointment>();
            AppointmentReceptionists = new HashSet<Appointment>();
            MedicalRecords = new HashSet<MedicalRecord>();
            Notifications = new HashSet<Notification>();
        }

        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime Dob { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int RoleId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int? StatusId { get; set; }
        public bool? Gender { get; set; }

        public virtual Role? Role { get; set; } = null!;
        public virtual UserStatus? Status { get; set; }
        public virtual Patient? Patient { get; set; }
        public virtual Staff? Staff { get; set; }
        public virtual ICollection<Appointment> AppointmentDoctors { get; set; }
        public virtual ICollection<Appointment> AppointmentReceptionists { get; set; }
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
