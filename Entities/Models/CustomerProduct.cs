using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class CustomerProduct : BaseEntity
    {
        public int Quantity { get; set; }
        public decimal? FinalPrice { get; set; }

        [StringLength(10)]
        public string DateAdded { get; set; }
        public int? StoreId { get; set; }
        [ForeignKey(nameof(Customer))]
        public int? CustomerId { get; set; }
        public User Customer { get; set; }
        [ForeignKey(nameof(Product))]
        public int? ProductId { get; set; }
        public Product Product { get; set; } 
        public List<CustomerAttributesProduct> CustomerAttributesProducts { get; set; }
    }
}
