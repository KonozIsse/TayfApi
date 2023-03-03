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
        public string CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
        public string DatePurchased { get; set; }
        public decimal OrderPrice { get; set; }  
        public decimal Total { get; set; }
        public string Notes { get; set; }
        public int DeliveryTimeId { get; set; }
        public int AddressId { get; set; }
        public string AddressName { get; set; }
        public string AddressDetail { get; set; }
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
        public string ShippingMethods { get; set; } 
        public decimal ShippingCost { get; set; }
        public decimal DisCount { get; set; }
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
        public string ProductName { get; set; }
        public string ProductImage { get; set; } 
        public string ProductModel{ get; set; }
        public decimal ProductPrice { get; set; }
        public List<OrderAttributProductDto> OrderAttributesProducts { get; set; }
    } 
    public class OrderAttributProductDto
    {
        public string Option { get; set; }
        public string Value { get; set; }
    }

    public class GoalCompletion

    {
        public int CartPercentage { get; set; }
        public int CompleteOrders { get; set; }
        public int PendingOrders { get; set; } 
        public int CanceledOrders { get; set; }
    }
}
