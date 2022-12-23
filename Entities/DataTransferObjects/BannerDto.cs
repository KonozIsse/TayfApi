using Entities.Models;
using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class BannerDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? LangId { get; set; }
        public int? VendorId { get; set; }
        public int ImgId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class UpdateBannerDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int? LangId { get; set; }
        public int ImgId { get; set; }
    }
}
