using System;
using System.Collections.Generic;

namespace Clinic_Management.Models
{
    public partial class Patient
    {
        public Patient()
        {
            Appointments = new HashSet<Appointment>();
            MedicalRecords = new HashSet<MedicalRecord>();
        }

        public int PatientId { get; set; }
        public int? NumberOfVisits { get; set; }
        public string? HealthInsurance { get; set; }

        public virtual User PatientNavigation { get; set; } = null!;
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; }
    }
}
