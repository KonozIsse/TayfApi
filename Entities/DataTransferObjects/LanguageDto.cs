using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models.Enums;

namespace Entities.DataTransferObjects
{
    public class LanguageDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public short? IsDefault { get; set; }
       public string Image { get; set; }
        public short Sort { get; set; }
    }

    public class UpdateLanguageDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; } 
        public string NameAr { get; set; }
        public string Direction { get; set; }
        public int? ImgId { get; set; }
    }
    
}
  

