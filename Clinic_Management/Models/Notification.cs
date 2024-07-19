using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Clinic_Management.Models
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public int ReceiverId { get; set; }
        public string? Content { get; set; }
        public string? Link { get; set; }
        public DateTime Datetime { get; set; }
        public bool? IsRead { get; set; }
        public string? Subject { get; set; }
        public int? Type { get; set; }
        [JsonIgnore]
        public virtual User Receiver { get; set; } = null!;
        
        public virtual NotificationType? TypeNavigation { get; set; }
    }
}
