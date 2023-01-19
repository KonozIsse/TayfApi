using Entities.Models.Enums;
using Entities.Models;
using System;
using System.Collections.Generic;

namespace Entities.DataTransferObjects
{
    public class ProductDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ImageProduct { get; set; }
        public string ProductModel { get; set; }
        public decimal Price { get; set; }
        public int Availability { get; set; }
        public int CountReviews { get; set; }
        public int NumLike { get; set; }
        public int? StoreId { get; set; }
        public int? AdminId { get; set; }
        public int TypeId { get; set; }
        public bool? IsAcceptAdmin { get; set; }
        public string StoreName { get; set; }
        public string StoreImage { get; set; }
        public string ShareLink { get; set; }
        public bool IsSpecial { get; set; }
        public bool IsSale { get; set; }
        public short IsFeature { get; set; }
        public decimal Rate { get; set; }
        public short IsBest { get; set; }
        public short IsPopular { get; set; }
        public int CategoryId { get; set; }
        public List<ProductCategoryDto> CategoriesName { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
        public List<WishListDto> WishLists { get; set; }
        public List<ReviewDto> Reviews { get; set; }
        public List<SpecialDto> SpecialProducts { get; set; }
        public List<SaleDto> ProductSales { get; set; }
        public List<AttributeDto> AttributesProducts { get; set; }
        public List<ImageDto> Images { get; set; }
    }
    public class RecentProductDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ImageProduct { get; set; }
        public decimal Price { get; set; }
    }
    public class CreateProductDto
    {
        public string ProductName { get; set; }
        public string ProductNameAr { get; set; }
        public Status IsStatus { get; set; }
        public string ProductModel { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string DescriptionAr { get; set; }
        public int TypeId { get; set; }
        public int? StoreId { get; set; } 
        public short IsFeature { get; set; }
        public short IsBest { get; set; }
        public short IsPopular { get; set; }
        public List<string> imagesProduct { get; set; }
        public bool IsSale { get; set; }
        public List<CreateSaleDto> ProductSales { get; set; }
        public bool IsSpecial { get; set; }
        public List<CreateSpecialDto> SpecialProducts { get; set; }
        public List<CategoriesProductDto> ProductCategories { get; set; }
    }
    public class UpdateProductDto : CreateProductDto
    {
        public int Id { get; set; }
    }
    public class ProductPageDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int? ImageId { get; set; }
        public decimal Price { get; set; }
        public decimal Rate { get; set; }
    }
    //Sales----------------------------------
    public class SaleDto
    {
        public int Id { get; set; }
        public decimal DiscountPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ProductId { get; set; }
    }
    public class CreateSaleDto 
    {
        public decimal DiscountPrice { get; set; }
        public Status IsStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    //Special----------------------------------
    public class SpecialDto
    {
        public int Id { get; set; }
        public decimal SpecialPrice { get; set; }
        public DateTime? EndDate { get; set; }
        public int ProductId { get; set; }
    }
    public class CreateSpecialDto
    {
        public decimal SpecialPrice { get; set; }
        public Status IsStatus { get; set; }
        public DateTime? EndDate { get; set; }
    } 

}
