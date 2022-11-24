using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class CartProduct : BaseEntity
    {
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Qty { get; set; }
        public int StoreId { get; set; }
        [ForeignKey(nameof(Cart))]
        public int? CartId { get; set; }
        public Cart Cart { get; set; }
        public List<CartAttributeProduct> CartAttributeProducts { get; set; }
    }
}
