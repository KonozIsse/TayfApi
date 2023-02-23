using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Notification : BaseEntity
    {
        public string Body { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public Nullable<int> NotificationActionId { get; set; }
        [ForeignKey(nameof(NotificationActionId))]
        public NotificationAction NotificationAction { get; set; }
        public NotificationStatus Status { get; set; }
        public string BodyAr { get; set; }
        public string SubjectAr { get; set; }
        public string Subject { get; set; }
        public string Data { get; set; }
        public Nullable<bool> IsRead { get; set; }
    }
}
