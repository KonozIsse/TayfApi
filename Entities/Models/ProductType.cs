using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class ProductType :BaseEntity
    {
        public string Type { get; set; }
        public List<Product> Products { get; set; }
    }
}
