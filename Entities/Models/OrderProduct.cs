namespace Entities.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    public class OrderProduct : BaseEntity
    {
        public int Qty { get; set; }
        public decimal FinalPrice { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public List<OrderAttributProduct> OrderAttributesProducts { get; set; }
    }
}
