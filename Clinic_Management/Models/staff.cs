using System;
using System.Collections.Generic;

namespace Clinic_Management.Models
{
    public partial class Staff
    {
        public int UserId { get; set; }
        public DateTime HireDate { get; set; }
        public string Cccd { get; set; } = null!;
        public string Image { get; set; } = null!;
        public int? DoctorDepartmentId { get; set; }
        public int? DoctorSpecialist { get; set; }

        public virtual Branch? DoctorDepartment { get; set; }
        public virtual Specialist? DoctorSpecialistNavigation { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
