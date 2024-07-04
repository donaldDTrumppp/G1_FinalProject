using System;
using System.Collections.Generic;

namespace Clinic_Management.Models
{
    public partial class Branch
    {
        public Branch()
        {
            Appointments = new HashSet<Appointment>();
            Staff = new HashSet<Staff>();
        }

        public int BranchId { get; set; }
        public string BranchName { get; set; } = null!;

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<Staff> Staff { get; set; }
    }
}
