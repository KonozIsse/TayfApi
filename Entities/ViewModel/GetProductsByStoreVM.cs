using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModel
{
    public class GetProductsByStoreVM
    {
        public List<ProductVM> Products { get; set; }
        public StoreDto Store { get; set; }
        public List<StoreDto> Stores { get; set; }
        public List<CategoryDto> Categories { get; set; }

    }
}
