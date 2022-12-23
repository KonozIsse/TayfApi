using Entities.Models;
using Entities.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class NotificationDto
    {
        public int UserId { get; set; }
        public Nullable<int> NotificationActionId { get; set; }
        public NotificationStatus Status { get; set; }
        public string Body { get; set; }
        public string BodyAr { get; set; }
        public string SubjectAr { get; set; }
        public string Subject { get; set; }
        public string Data { get; set; }
        public Nullable<bool> IsRead { get; set; }
    }
    public class CreateNotificationDto
    {
        public string Body { get; set; }
        public string BodyAr { get; set; }
        public string SubjectAr { get; set; }
        public string Subject { get; set; }
    }
}
