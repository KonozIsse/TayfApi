using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class CustomerAttributesProduct : BaseEntity
    {
        [ForeignKey(nameof(CustomerProduct))]
        public int? CustomerProductId { get; set; }
        public CustomerProduct CustomerProduct { get; set; }

        [ForeignKey(nameof(AttributesProduct))]
        public int? AttributesProductId { get; set; }
        public ProductAttribut AttributesProduct { get; set; }
    }
}
