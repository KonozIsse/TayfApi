using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class AttributeDto
    {
        public int Id { get; set; }
        public string PricePrefix { get; set; }
        public decimal AttributePrice { get; set; }
        public string ProductName { get; set; }
        public string Option { get; set; }
        public OptionType OptionType { get; set; }
        public string Value { get; set; }
    }
    public class CreateAttributeDto
    {
        [Required]
        public string PricePrefix { get; set; }
        [Required]
        public decimal AttributePrice { get; set; }
        [Required]
        public short IsDefault { get; set; }
        [Required]
        public int OptionId { get; set; }
        [Required]
        public int ValueId { get; set; }
    }
    public class UpdateAttributeDto : CreateAttributeDto
    {
        public int Id { get; set; }
    }
  
    public class OptionDto
    {
        public int Id { get; set; }
        public string OptionName { get; set; }
        public OptionType OptionType { get; set; }
        public List<ValueVM> Values { get; set; }
    }

    public class CreateOptionDto
    {
        [Required]
        public string OptionName { get; set; }
        [Required]
        public OptionType OptionType { get; set; }
    } 
    public class UpdateOptionDto : CreateOptionDto
    {
        public int Id { get; set; }
    }
    public class ValueDto
    {
        public int Id { get; set; }
        public string ValueHexModel { get; set; }
        public string OptionValueName { get; set; }
        public int OptionId { get; set; }
        public string OptionName { get; set; }
        public OptionType OptionType { get; set; }
    }
    public class CreateValueDto
    {
        [Required]
        public string ValueHexModel { get; set; }
        public string OptionValueName { get; set; }
    }
    public class UpdateValueDto : CreateValueDto
    {
        public int Id { get; set; }
    }

    public class ValueVM
    {
        public Nullable<int> AttributeId { get; set; }
        public int ValueId { get; set; }
        public string ValueHexModel { get; set; }
        public string OptionValueName { get; set; }
        public int OptionId { get; set; }
        public string OptionName { get; set; }
        public decimal AttributePrice { get; set; }
        public short IsDefault { get; set; }

    }
}
