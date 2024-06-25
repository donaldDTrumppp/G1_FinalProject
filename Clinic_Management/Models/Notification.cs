using System;
using System.Collections.Generic;

namespace Clinic_Management.Models
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public int ReceiverId { get; set; }
        public string? Content { get; set; }
        public string? Link { get; set; }
        public DateTime Datetime { get; set; }

        public virtual User Receiver { get; set; } = null!;
    }
}
