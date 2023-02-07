namespace Entities.Models
{
    using Entities.Models.Enums;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Product : BaseEntity
    {
        public string ProductName { get; set; }
        public string ProductNameAr { get; set; }
        public string Description { get; set; }
        public string DescriptionAr { get; set; }
        public string ProductModel { get; set; }
        public decimal Price { get; set; }
        public int Availability { get; set; }
        public bool IsSpecial { get; set; }
        public bool IsSale { get; set; }
        public short IsFeature { get; set; }
        public short IsBest { get; set; }
        public short IsPopular { get; set; }
        public int? AdminId { get; set; }
        [ForeignKey(nameof(Store))]
        public int StoreId { get; set; }
        public User Store { get; set; }
        public bool? IsAcceptAdmin { get; set; }
        [ForeignKey(nameof(ProductType))]
        public int TypeId { get; set; }
        public ProductType ProductType { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        public List<WishList> WishLists { get; set; }
        public List<Review> Reviews { get; set; }
        public List<SpecialProducts> SpecialProducts { get; set; } 
        public List<ProductSales> ProductSales { get; set; }
        public List<ProductAttribut> AttributesProducts { get; set; }
        public List<OrderProduct> OrderProducts { get; set; }
        public List<ProductImage> Images { get; set; }
        public List<Cart> Carts { get; set; }

    }
}
