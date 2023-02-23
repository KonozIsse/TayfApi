using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class NotificationAction : BaseEntity
    {
        public NotificationKey NotificationKey { get; set; }
        public string Template { get; set; }
        public string Subject { get; set; }
        public string TemplateAr { get; set; }
        public string SubjectAr { get; set; }
    }
}
