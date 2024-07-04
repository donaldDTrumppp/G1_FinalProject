using System;
using System.Collections.Generic;

namespace Clinic_Management.Models
{
    public partial class Specialist
    {
        public Specialist()
        {
            Appointments = new HashSet<Appointment>();
            Staff = new HashSet<Staff>();
        }

        public int SpecialistId { get; set; }
        public string SpecialistName { get; set; } = null!;

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<Staff> Staff { get; set; }
    }
}
