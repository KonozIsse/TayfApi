using Entities.Models.Enums;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;

namespace Entities.DataTransferObjects
{
    public class ProductDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ProductModel { get; set; }
        public decimal Price { get; set; }
        public int Availability { get; set; }
        public ProductType ProductType { get; set; }
        public bool IsSpecial { get; set; }
        public bool IsSale { get; set; }
        public short IsFeature { get; set; }
        public bool? IsFavorite { get; set; }
        public decimal Rate { get; set; }
        public short IsBest { get; set; }
        public short IsPopular { get; set; }
        public bool? IsAcceptAdmin { get; set; }
        public int? TopRateProductId { get; set; }
        public int CategoryId { get; set; }
        public List<WishListDto> WishLists { get; set; }
        public List<ReviewDto> Reviews { get; set; }
        public List<SpecialProductsDto> SpecialProducts { get; set; }
        public List<ProductSalesDto> ProductSales { get; set; }
        public List<AttributeDto> AttributesProducts { get; set; }
        public List<ProductsStoreDto> ProductsStores { get; set; }
        public List<ImageDto> Images { get; set; }
    }
    public class SpecialProductsDto
    {
        public int Id { get; set; }
        public decimal SpecialPrice { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime? EndDate { get; set; }
        public int ProductId { get; set; }
    }
    public class ProductSalesDto
    {
        public int Id { get; set; }
        public decimal DiscountPrice { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ProductId { get; set; }
    }
    public class ProductPageDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int? ImageId { get; set; }
        public decimal Price { get; set; }
        public decimal Rate { get; set; }
    }
    public class CreateProductDto
    {
        public string ProductName { get; set; }
        public string ProductModel { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int TypeId { get; set; }
        public short IsFeature { get; set; }
        public short IsBest { get; set; }
        public short IsPopular { get; set; }
        public int? TopRateProductId { get; set; }
        public int? ImgId { get; set; }
    }
    public class UpdateProductDto : CreateProductDto
    {
    }  
    public class UpdateSalesProductDto 
    {
        public decimal DiscountPrice { get; set; }
        public DateTime? LastUpdate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
    public class CreateProductSalesDto 
    {
        public int ProductId { get; set; }
        public decimal SpecialPrice { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }  
    public class CreateSpecialProductsDto
    {
        public decimal SpecialPrice { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime? EndDate { get; set; }
    } 
    public class UpdateSpecialProductDto  : CreateSpecialProductsDto
    {
    } 

   
}
