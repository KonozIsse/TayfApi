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
    public class ServiceDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; } 
        public string CreatedAt { get; set; } 
        public string UpdatedAt { get; set; }
    } 
   
    public class UpdateServiceDto
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string TitleAr { get; set; }
        [Required]
        public string DescriptionAr { get; set; }
        public int? ImgId { get; set; }
    }
}
