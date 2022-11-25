namespace Entities.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Cart : BaseEntity
    {
        //[ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Qty { get; set; }
        public decimal FinalPrice { get; set; }
       // [ForeignKey(nameof(Store))]
        public int StoreId { get; set; }
      //  public User Store { get; set; }
       
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public User Customer { get; set; }
        public List<CartAttributeProduct> CartAttributeProducts { get; set; }
    }
}
