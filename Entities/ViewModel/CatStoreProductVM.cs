using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModel
{
    public class CatStoreProductVM
    {
        public List<CategoryDto> Categories { get; set; }
        public List<ProductDto> Products { get; set; }
        public List<StoreDto> Stores { get; set; }
    }
}
