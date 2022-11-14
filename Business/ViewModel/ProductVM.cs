using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ViewModel
{
    public class ProductVM
    {
        public int MainCategoryId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryImage { get; set; }
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ProductModel { get; set; }
        public int TypeId { get; set; }
        public List<String> images { get; set; }
        public string ProductImage { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductStatus { get; set; }
        public int AvailabilityProduct { get; set; }
        public List<OptionDto> Options { get; set; }
        public string ShareLink { get; set; }

        public bool isFlash { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? expireDate { get; set; }
        public bool is_special { get; set; }
        public decimal offer_price { get; set; }
        public decimal flash_price { get; set; }

        public short IsFeature { get; set; }
        public short IsBest { get; set; }
        public short IsPopular { get; set; }

        public int likeId { get; set; }
        public bool IsFav { get; set; }
        public bool IsReview { get; set; }
        public List<ReviewDto> Reviews { get; set; }
        public Nullable<decimal> Rate { get; set; }

        public Nullable<int> StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreImage { get; set; }

    }
}
//public Dictionary<string, string> ProductNames { get; set; }
//public Dictionary<string, string> ProductDescriptions { get; set; }