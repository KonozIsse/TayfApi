using Entities.Models.Enums;
using ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
namespace Entities.DataTransferObjects
{
    public class CountryDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string CountryName { get; set; }
        public string CountryCode2 { get; set; }
        public string CountryCode3 { get; set; }
        public string MobileCode { get; set; }
        public string Image { get; set; }
    }
    [EnumBindResource(typeof(SharedResource))]
    public class CreateCountryDto
    {
        //[Required(ErrorMessageResourceName = "enterallfiled", ErrorMessageResourceType = typeof(ResourcesLib.SharedResource))]
        // [Required(ErrorMessageResourceName = "enterallfiled", ErrorMessageResourceType = typeof(SharedResource))]
        [Required(ErrorMessage = "enterallfiled")]
         public string CountryNameAr { get; set; }
        // [Required(ErrorMessageResourceType = (typeof(ResourcesLib.SharedResource)), ErrorMessageResourceName = "enterallfiled")]
        [Required(ErrorMessage = "enterallfiled")]
        public string CountryName { get; set; }
        [Required]
        public string CountryCode2 { get; set; }
        [Required]
        public string CountryCode3 { get; set; }
        [Required]
        public string MobileCode { get; set; }
        [Required(ErrorMessage = "correctImage")]
        // [Required(ErrorMessageResourceType = (typeof(ResourcesLib.SharedResource)), ErrorMessageResourceName = "correctImage")]
        public int ImageId { get; set; }
    }
    public class UpdateCountryDto: CreateCountryDto
    {
        public int Id { get; set; }
    }
}
