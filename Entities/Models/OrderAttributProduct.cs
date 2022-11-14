using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class OrderAttributProduct : BaseEntity
    {

        [ForeignKey(nameof(OrderProducts))]
        public int? OrderProductsId { get; set; }
        public OrderProduct OrderProducts { get; set; }

        [ForeignKey(nameof(ProductAttribut))]
        public int? ProductAttributId { get; set; }
        public ProductAttribut ProductAttribut { get; set; }
    }
}
