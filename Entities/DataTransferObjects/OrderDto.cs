using Entities.Models;
using Entities.Models.Enums;
using Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class OrderDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public decimal TotalTax { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
        public string DatePurchased { get; set; }
        public decimal OrderPrice { get; set; }
        public string TransactionId { get; set; }
        public string HashedCtpAndPayment { get; set; }
        public string Notes { get; set; }
        public int? ShippingMethodId { get; set; }
        public int DeliveryTimeId { get; set; }
        public int? CouponId { get; set; }
        public int PaymentMethodsId { get; set; }
        public int AddressId { get; set; }
        public int CustomerId { get; set; }
        public int OrderStatusId { get; set; }
        public int CurrencyId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreEmail { get; set; }
        public string StorePhone { get; set; }
        public string OrderStatusName { get; set; }
        public string Currency { get; set; }
        public int CountProduct { get; set; }
        public string DeliveryTimeName { get; set; }
        public List<OrderProductDto> OrderProducts { get; set; }
    } 
    public class CreateOrderDto
    {
        public string Notes { get; set; }
        public int DeliveryTimeId { get; set; }
        public int AddressId { get; set; }
        public string CouponCode { get; set; }
    }
    public class UpdateOderDto : CreateOrderDto
    {
    }
    public class OrderProductDto
    {
        public int Qty { get; set; }
        public int? OrderId { get; set; }
        public int ProductId { get; set; }
        public List<OrderAttributProductDto> OrderAttributesProducts { get; set; }
    } 
    public class OrderAttributProductDto
    {
        public int? OrderProductsId { get; set; }
        public int? ProductAttributId { get; set; }
    }
    public class InvoiceOrderVM
    {
        public int Id { get; set; }
        public decimal TotalTax { get; set; }
        public decimal CouponAmount { get; set; }
        public decimal OrderPrice { get; set; }
        public DateTime CreateAt { get; set; }
        public string CouponCode { get; set; }
        public List<OrderProductsDto> OrderProducts { get; set; }
        public User Customer { get; set; }
        public AddressDto Address { get; set; }
        public DeliveryTime Time { get; set; }
    }
    public class OrderProductsDto
    {
        public int Qty { get; set; }
        public int? OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductModel { get; set; }
        public string ProductImage { get; set; }
        public decimal ProductPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProductName { get; set; }
        public List<OptionDto> Options { get; set; }
    }
}
