using Entities.Models.Enums;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string IsStatus { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ImageProduct { get; set; }
        public string ProductModel { get; set; }
        public decimal Price { get; set; }
        public int Availability { get; set; }
        public int CountReviews { get; set; }
        public int NumLike { get; set; }
        public int StoreId { get; set; }
        public int AdminId { get; set; }
        public string StoreName { get; set; }
        public string StoreImage { get; set; }
        public ProductsType ProductType { get; set; }
        public string ShareLink { get; set; }
        public bool IsFavorite { get; set; } 
        public short IsFeature { get; set; }
        public decimal Rate { get; set; }
        public short IsBest { get; set; }
        public short IsPopular { get; set; }
        public bool IsSpecial { get; set; }
        public bool IsSale { get; set; }
        public bool IsAcceptAdmin { get; set; }
        public string CategoryName { get; set; }
        public string CategoryImage { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
        public List<ReviewDto> Reviews { get; set; }
        public List<SpecialDto> SpecialProducts { get; set; }
        public List<SaleDto> ProductSales { get; set; }
        public List<AttributeDto> AttributesProducts { get; set; }
        public List<ProductCategoryDto> ProductCategories { get; set; }
        public List<ProductImagesDto> Images { get; set; }
    }
    public class CreateProductDto
    {
        [Required(ErrorMessage = "enterallfiled")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "enterallfiled")]
        public string ProductNameAr { get; set; }
        [Required]
        public Status IsStatus { get; set; }
        [Required]
        public string ProductModel { get; set; }
        [Required(ErrorMessage = "enterPrice")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "enterallfiled")]
        public string Description { get; set; }
        [Required(ErrorMessage = "enterallfiled")]
        public string DescriptionAr { get; set; }
        [Required]
        public ProductsType Type { get; set; }
        public int Availability { get; set; }
        [Required]
        public int StoreId { get; set; } 
        public short IsFeature { get; set; }
        public short IsBest { get; set; }
        public short IsPopular { get; set; } 
        public short IsSale { get; set; }
        public short IsSpecial { get; set; }
        //[Required(ErrorMessage = "correctImage")]
        public List<CreateImageProductDto> Images { get; set; }
        [Required(ErrorMessage = "selectcategory")]
        public List<CreateProductCategoryDto> ProductCategories { get; set; }
        //-----------------------
        public decimal DiscountPrice { get; set; }
        public Status IsStatusSale { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        //----------------------
        public decimal SpecialPrice { get; set; }
        public Status IsStatusSpecial { get; set; }
        public DateTime? EndDateSpecial { get; set; }
    }
    public class UpdateProductDto : CreateProductDto
    {
        public int Id { get; set; }
    }
    //image---------------------------------- 
    public class CreateImageProductDto
    {
        public int Id { get; set; }
        public int ImageId { get; set; }
    }
    public class ProductImagesDto
    {
        public string Image { get; set; }
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
    //Special----------------------------------
    public class SpecialDto
    {
        public int Id { get; set; }
        public decimal SpecialPrice { get; set; }
        public DateTime? EndDate { get; set; }
        public int ProductId { get; set; }
    }

}
