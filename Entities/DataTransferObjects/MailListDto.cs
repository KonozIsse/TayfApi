using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class MailListDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string Email { get; set; }  
        public DateTime CreatedAt { get; set; }
    }
    public class SendMailListDto
    {
        public string Email { get; set; }
    }
    public class MessageTemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
    public class UpdateTemplateDto
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
