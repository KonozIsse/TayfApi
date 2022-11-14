using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class CartStore : BaseEntity
    {
        [ForeignKey(nameof(Store))]
        public int StoreId { get; set; }
        public User Store { get; set; } 
        [ForeignKey(nameof(Cart))]
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
