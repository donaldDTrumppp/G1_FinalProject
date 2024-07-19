using System;
using System.Collections.Generic;

namespace Clinic_Management.Models
{
    public partial class Appointment
    {
        public int AppointmentId { get; set; }
        public int BranchId { get; set; }
        public int? PatientId { get; set; }
        public string PatientName { get; set; } = null!;
        public DateTime PatientDob { get; set; }
        public string PatientPhoneNumber { get; set; } = null!;
        public string PatientEmail { get; set; } = null!;
        public string PatientAddress { get; set; } = null!;
        public int? DoctorId { get; set; }
        public int? ReceptionistId { get; set; }
        public DateTime RequestedTime { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? RealTimeShowUp { get; set; }
        public int? Specialist { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
        public bool? Reminded { get; set; }

        public virtual Branch Branch { get; set; } = null!;
        public virtual User? Doctor { get; set; }
        public virtual Patient? Patient { get; set; }
        public virtual User? Receptionist { get; set; }
        public virtual Specialist? SpecialistNavigation { get; set; }
        public virtual AppointmentStatus StatusNavigation { get; set; } = null!;
        public virtual MedicalRecord? MedicalRecord { get; set; }
    }
}
