using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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
        public int? MobileCode { get; set; }
        public int ImageId { get; set; }
    }
    public class CreateCountryDto
    {
        public Dictionary<string, string> CountryNames { get; set; }
        public string CountryNameAr { get; set; }
        public string CountryName { get; set; }
        public string CountryCode2 { get; set; }
        public string CountryCode3 { get; set; }
        public int? MobileCode { get; set; }
        public int ImageId { get; set; }
    }
    public class UpdateCountryDto: CreateCountryDto
    {
        public int Id { get; set; }
    }
}
