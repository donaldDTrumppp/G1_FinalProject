using System;
using System.Collections.Generic;

namespace Clinic_Management.Models
{
    public partial class User
    {
        public User()
        {
            AppointmentDoctors = new HashSet<Appointment>();
            AppointmentPatients = new HashSet<Appointment>();
            AppointmentReceptionists = new HashSet<Appointment>();
            MedicalRecordDoctors = new HashSet<MedicalRecord>();
            MedicalRecordPatients = new HashSet<MedicalRecord>();
            Notifications = new HashSet<Notification>();
        }

        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime Dob { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int RoleId { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual staff? staff { get; set; }
        public virtual ICollection<Appointment> AppointmentDoctors { get; set; }
        public virtual ICollection<Appointment> AppointmentPatients { get; set; }
        public virtual ICollection<Appointment> AppointmentReceptionists { get; set; }
        public virtual ICollection<MedicalRecord> MedicalRecordDoctors { get; set; }
        public virtual ICollection<MedicalRecord> MedicalRecordPatients { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
