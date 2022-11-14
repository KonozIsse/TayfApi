
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class ProductAttribut : BaseEntity
    {
        [Required]
        [StringLength(1)]
        public string PricePrefix { get; set; }
        public decimal AttributePrice { get; set; }
        public short IsDefault { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [ForeignKey(nameof(ProductOption))]
        public int OptionId { get; set; }
        public ProductOption ProductOption { get; set; }

        [ForeignKey(nameof(ProductOptionValue))]
        public int ValueId { get; set; }
        public ProductOptionValue ProductOptionValue { get; set; }
        public List<OrderAttributProduct> OrderAttributesProducts { get; set; }
    }
}