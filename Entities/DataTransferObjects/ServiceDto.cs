using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class ServiceDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? ImgId { get; set; } 
        public DateTime CreatedAt { get; set; }
    } 
   
    public class UpdateServiceDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? ImgId { get; set; }
    }
}
