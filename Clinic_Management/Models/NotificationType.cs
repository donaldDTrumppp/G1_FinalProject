using System;
using System.Collections.Generic;

namespace Clinic_Management.Models
{
    public partial class NotificationType
    {
        public NotificationType()
        {
            Notifications = new HashSet<Notification>();
        }

        public int TypeId { get; set; }
        public string? TypeName { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
