using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class ProductImage : BaseEntity
    {
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }  
        public Product Product { get; set; }
        [ForeignKey(nameof(Image))]
        public int ImageId { get; set; } 
        public Image Image { get; set; }
    }
}
