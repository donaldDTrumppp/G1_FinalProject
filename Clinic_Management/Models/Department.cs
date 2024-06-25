using System;
using System.Collections.Generic;

namespace Clinic_Management.Models
{
    public partial class Department
    {
        public Department()
        {
            Appointments = new HashSet<Appointment>();
            staff = new HashSet<staff>();
        }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<staff> staff { get; set; }
    }
}
