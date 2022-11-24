namespace Entities.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Cart : BaseEntity
    {
        public decimal FinalPrice { get; set; }
        public string Notes { get; set; }
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public User Customer { get; set; }
        public List<CartProduct> CartProducts { get; set; }
    }
}
