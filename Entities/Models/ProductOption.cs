using System.Collections.Generic;

namespace Entities.Models
{
    public class ProductOption : BaseEntity 
    {
        public string OptionName { get; set; }
        public string OptionType { get; set; }
        public List<ProductOptionValue> Values { get; set; }
    }
}