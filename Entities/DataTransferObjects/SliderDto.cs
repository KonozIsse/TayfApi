using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class SliderDto
    {
        public int Id { get; set; }
         public string Image { get; set; }
        public string Title { get; set; }
        public string Decription { get; set; }
       
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
    public class CreateSliderDto
    {
        [Required]
        public int ImageId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Decription { get; set; }
        [Required]
        public string TitleAr { get; set; }
        [Required]
        public string DecriptionAr { get; set; }
        public string Url { get; set; }
        public int? LangId { get; set; }
    }
    public class UpdateSliderDto : CreateSliderDto
    {
        public string Image { get; set; }
        public int Id { get; set; }
    }
}
