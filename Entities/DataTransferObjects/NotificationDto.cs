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
        public int Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public DateTime CreatedAt { get; set; }
        public NotificationKey NotificationKey { get; set; }
    }
    public class CreateNotificationDto
    {
        public string Body { get; set; }
        public string BodyAr { get; set; }
        public string SubjectAr { get; set; }
        public string Subject { get; set; }
        public List<int> IdUsers { get; set; } 
    }
}
