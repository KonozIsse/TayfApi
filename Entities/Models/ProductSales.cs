namespace Entities.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    public class ProductSales: BaseEntity
    {
        public decimal DiscountPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
