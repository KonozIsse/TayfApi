namespace Entities.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    public class WishList : BaseEntity
    {
        [ForeignKey(nameof(Product))]
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public User Customer { get; set; }
    }
}
