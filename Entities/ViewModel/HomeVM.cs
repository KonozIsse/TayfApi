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
        public int CustomerId { get; set; }
        public LanguageDto Language { get; set; }
        public CurrencyDto Currency { get; set; }
        public List<MainCategoryDto> categories { get; set; }
        public List<SliderDto> sliders { get; set; }
        public List<ServiceDto> services { get; set; }
        public List<NewsDto> blog { get; set; }
        public List<ProductDto> products { get; set; }
        public List<ProductDto> flash { get; set; }
        public List<ProductDto> specialProducts { get; set; }
        public List<StoreDto> stores { get; set; }
        public List<SettingDto> settings { get; set; }
        public BannerDto Banner { get; set; }
        public List<ProductDto> ProductsPopular { get; set; }
        public List<ProductDto> ProductsBest { get; set; }
        public List<ProductDto> ProductsLatest { get; set; }
        public List<ProductDto> ProductsSpecial { get; set; }
        public List<ProductDto> ProductsTopRated { get; set; }
        public List<ProductDto> ProductsDailyDeal { get; set; }
    }
}
