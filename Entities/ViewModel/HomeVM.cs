using Entities.DataTransferObjects;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModel
{
    public class HomeVM
    {
        public List<SliderDto> Sliders { get; set; }
        public List<ServiceDto> Services { get; set; }
        public List<NewsDto> Blogs { get; set; }
        public List<ProductDto> Products { get; set; }
        public List<ProductDto> FlashProducts { get; set; }
        public List<ProductDto> SpecialProducts { get; set; }
        public List<StoreDto> Stores { get; set; }
        public BannerDto Banner { get; set; }
        public List<ProductDto> ProductsPopular { get; set; }
        public List<ProductDto> ProductsBest { get; set; }
        public List<ProductDto> ProductsLatest { get; set; }
        public List<ProductDto> ProductsTopRated { get; set; }
        public List<ProductDto> ProductsDailyDeal { get; set; }
    }
}
