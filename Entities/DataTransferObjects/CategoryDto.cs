using Entities.Models;
using Entities.Models.Enums;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public int ImageId { get; set; }
        public Status IsStatus { get; set; }
        public string CategoryName { get; set; }
        public int? MainCategoryId { get; set; }
        public int? CountProduct { get; set; }
        public int? Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<ProductDto> Products { get; set; }
    }
    public class MainCategoryDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int ImageId { get; set; }
        public Status IsStatus { get; set; }
    }
    public class CreateCategoryDto
    {
        public Dictionary<string, string> CategoryNames { get; set; }
        public int? MainCategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? ImgId { get; set; }
        public Status IsStatus { get; set; }
    }
    public class UpdateCategoryDto : CreateCategoryDto
    {
        public int Id { get; set; }
    }
}
