using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class ProductOptionValue : BaseEntity
    {
        public string ValueHexModel { get; set; }
        public string OptionValueName { get; set; }
        [ForeignKey(nameof(ProductOption))]
        public int OptionId { get; set; }
        public ProductOption ProductOption { get; set; }
    }
}