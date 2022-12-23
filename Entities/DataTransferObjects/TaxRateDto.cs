using Entities.Models;
using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class TaxRateDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public decimal Tax_Rate { get; set; }
        public int TaxClassId { get; set; }
        public int ZoneId { get; set; } 
        public int? StoreId { get; set; }
    }
    public class CreateTaxRateDto
    {
        public decimal Tax_Rate { get; set; }
        public string Description { get; set; }
        public int TaxClassId { get; set; }
        public int ZoneId { get; set; }
    }
    public class UpdateTaxRateDto : CreateTaxRateDto
    {
        public int Id { get; set; }
    } 
    public class TaxClassDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } 
        public DateTime CreateAt { get; set; }
        public int? StoreId { get; set; }
    }
    public class CreateTaxClassDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
    } 
    public class UpdateTaxClassDto: CreateTaxClassDto
    {
        public int Id { get; set; }
    }
}
