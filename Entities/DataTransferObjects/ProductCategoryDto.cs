using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class ProductCategoryDto
    {
        public int? MainCategoryId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } 
        public string CategoryImage { get; set; }
    } 
    public class CreateProductCategoryDto
    {
        public int CategoryId { get; set; }
    }
}
