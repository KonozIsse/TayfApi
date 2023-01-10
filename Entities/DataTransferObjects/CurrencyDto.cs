using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class CurrencyDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Symbol { get; set; }
        public string DecimalPlaces { get; set; }
        public double? Value { get; set; }
        public int IsDefault { get; set; }
    }
    public class CreateCurrencyDto
    {
        public Status IsStatus { get; set; }
        public string Name { get; set; } 
        public string NameAr { get; set; }
        public string Position { get; set; }
        public string Symbol { get; set; }
        public string DecimalPlaces { get; set; }
        public double? Value { get; set; }
    }
    public class UpdateCurrencyDto : CreateCurrencyDto
    {
        public int Id { get; set; }
    }
}
