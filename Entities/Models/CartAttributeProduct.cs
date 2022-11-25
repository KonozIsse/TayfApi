using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class CartAttributeProduct : BaseEntity
    {
        //[ForeignKey(nameof(Cart))]
        public int? CartId { get; set; }
       // public Cart Cart { get; set; }

        [ForeignKey(nameof(AttributesProduct))]
        public int AttributesProductId { get; set; }
        public ProductAttribut AttributesProduct { get; set; }
    }
}
