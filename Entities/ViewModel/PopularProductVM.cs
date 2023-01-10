using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModel
{
    public class PopularProductVM
    {
        public List<ProductVM> products { get; set; }
        public List<CategoryDto> categories { get; set; }
        public List<StoreDto> stores { get; set; }

    }
}
