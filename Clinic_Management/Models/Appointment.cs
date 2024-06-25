using System;
using System.Collections.Generic;

namespace Clinic_Management.Models
{
    public partial class Appointment
    {
        public Appointment()
        {
            MedicalRecords = new HashSet<MedicalRecord>();
        }

        public int AppointmentId { get; set; }
        public int DepartmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = null!;
        public DateTime PatientDob { get; set; }
        public string PatientPhoneNumber { get; set; } = null!;
        public string PatientEmail { get; set; } = null!;
        public string PatientAddress { get; set; } = null!;
        public int? DoctorId { get; set; }
        public int? ReceptionistId { get; set; }
        public DateTime RequestedTime { get; set; }
        public int? Specialist { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }

        public virtual Department Department { get; set; } = null!;
        public virtual User? Doctor { get; set; }
        public virtual User Patient { get; set; } = null!;
        public virtual User? Receptionist { get; set; }
        public virtual Specialist? SpecialistNavigation { get; set; }
        public virtual AppointmentStatus StatusNavigation { get; set; } = null!;
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; }
    }
}
