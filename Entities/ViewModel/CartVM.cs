using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModel
{
    public class CartVM
    {
        public int Id { get; set; }
        public int Qty { get; set; }
        public decimal FinalPrice { get; set; }
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }

        public bool IsSpecial { get; set; }
        public decimal SpecialPrice { get; set; }
        public string ProductModel { get; set; }
        public string ProductImage { get; set; }
        public decimal ProductPrice { get; set; }
        public bool? IsFavorite { get; set; }
        public short ProductStatus { get; set; }
        public decimal TotaLTax { get; set; }
        public short IsFeature { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string ProductName { get; set; }
        public string ShareLink { get; set; }
        public string ProductDescription { get; set; }
        //public string CategoryName { get; set; }
        //public Nullable<int> CategoryId { get; set; }
        public List<AttributeDto> Attributes { get; set; }
        public decimal? Rating { get; set; }
    }
}
