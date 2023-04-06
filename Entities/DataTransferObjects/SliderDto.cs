using Entities.Models.Enums;
using System;
using System.Collections.Generic;
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
        public int ImageId { get; set; }
        public string Title { get; set; }
        public string Decription { get; set; }
        public string TitleAr { get; set; }
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
