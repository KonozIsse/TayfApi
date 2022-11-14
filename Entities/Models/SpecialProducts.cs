namespace Entities.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class SpecialProducts : BaseEntity
    {
        public decimal SpecialPrice { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime? EndDate { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
