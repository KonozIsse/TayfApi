using Entities.Models;
using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public NotificationKey NotificationKey { get; set; } 
        public DateTime CreatedAt { get; set; }

    }
    public class CreateNotificationDto
    {
        [Required]
        public string Body { get; set; }
        [Required]
        public string BodyAr { get; set; }
        [Required]
        public string SubjectAr { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public List<int> IdUsers { get; set; } 
    }
}
