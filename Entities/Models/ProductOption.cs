using Entities.Models.Enums;
using System.Collections.Generic;

namespace Entities.Models
{
    public class ProductOption : BaseEntity 
    {
        public string OptionName { get; set; }
        public OptionType OptionType { get; set; }
        public List<ProductOptionValue> Values { get; set; }
    }
}