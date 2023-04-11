namespace Entities.Models
{
    using System.ComponentModel.DataAnnotations;
    public class ShippingMethods : BaseEntity
    {
        [Required]
        [StringLength(191)]
        public string Name { get; set; }
        public string ShippingMethod { get; set; }
        public decimal ShippingCost { get; set; }
    }
}
