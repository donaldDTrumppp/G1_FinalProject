using System;
using System.Collections.Generic;

namespace Clinic_Management.Models
{
    public partial class Branch
    {
        public Branch()
        {
            Appointments = new HashSet<Appointment>();
            staff = new HashSet<staff>();
        }

        public int BranchId { get; set; }
        public string BranchName { get; set; } = null!;

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<staff> staff { get; set; }
    }
}
