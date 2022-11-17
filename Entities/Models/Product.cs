namespace Entities.Models
{
    using Entities.Models.Enums;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Product : BaseEntity
    {
        public string ProductName { get; set; }
        public string ProductModel { get; set; }
        public decimal Price { get; set; }
        public int Availability { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
        public int CountReviews { get; set; }
        public int Like { get; set; }
        public bool IsSpecial { get; set; }
        public bool IsSale { get; set; }
        public bool? IsFavorite { get; set; }
        public short IsFeature { get; set; }
        public short IsBest { get; set; }
        public short IsPopular { get; set; }
        [ForeignKey(nameof(Vendor))]
        public int VendorId { get; set; }
        public User Vendor { get; set; }
        public bool? IsAcceptAdmin { get; set; }
        [ForeignKey(nameof(ProductType))]
        public int TypeId { get; set; }
        public ProductType ProductType { get; set; }

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<WishList> WishLists { get; set; }
        public List<Review> Reviews { get; set; }
        public List<SpecialProducts> SpecialProducts { get; set; } 
        public List<CustomerProduct> CustomerProducts { get; set; }
        public List<ProductSales> ProductSales { get; set; }
        public List<ProductAttribut> AttributesProducts { get; set; }
        public List<CartProduct> CartProducts { get; set; }
        public List<OrderProduct> OrderProducts { get; set; }
        public List<Image> Images { get; set; }

    }
}
