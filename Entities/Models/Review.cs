namespace Entities.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    
    public class Review :  BaseEntity
    {
        public string Text { get; set; }
        public string CustomerName { get; set; }
        public double Rating { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public User Customer { get; set; }
    }
}
