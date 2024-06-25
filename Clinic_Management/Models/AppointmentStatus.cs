using System;
using System.Collections.Generic;

namespace Clinic_Management.Models
{
    public partial class AppointmentStatus
    {
        public AppointmentStatus()
        {
            Appointments = new HashSet<Appointment>();
        }

        public int StatusId { get; set; }
        public string StatusName { get; set; } = null!;

        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
