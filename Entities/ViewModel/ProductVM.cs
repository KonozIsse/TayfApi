using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModel
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
        public List<String> Images { get; set; }
        public string ProductImage { get; set; }
        public decimal Price { get; set; }
        public string IsStatus { get; set; }
        public int Availability { get; set; }
        public string ShareLink { get; set; }
        public short IsFeature { get; set; }
        public short IsBest { get; set; }
        public short IsPopular { get; set; }
        public List<OptionDto> Attributs { get; set; }

        public bool IsSale { get; set; }
        public decimal DiscountPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsSpecial { get; set; }
        public decimal SpecialPrice { get; set; }

        public int LikeId { get; set; }
        public int NumLike { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsReview { get; set; }
        public List<ReviewDto> Reviews { get; set; }
        public Nullable<decimal> Rate { get; set; }

        public Nullable<int> StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreImage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}