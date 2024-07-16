using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Clinic_Management.Models
{
    public partial class MedicalRecord
    {
        public int MedicalrecordId { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int? DoctorId { get; set; }

        [DisplayName("Visit time")]
        public DateTime VisitTime { get; set; }
        public string Symptoms { get; set; } = null!;
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }

        public virtual Appointment? Appointment { get; set; } = null!;
        public virtual User? Doctor { get; set; }
        public virtual User? Patient { get; set; } = null!;
    }
}
