using Entities.Models;
using Entities.Models.Enums;
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
        public DateTime? DatePurchased { get; set; }
        public decimal OrderPrice { get; set; }
        public string TransactionId { get; set; }
        public string HashedCtpAndPayment { get; set; }
        public string Notes { get; set; }
        public int? ShippingMethodId { get; set; }
        public int DeliveryTimeId { get; set; }
        public int? CouponId { get; set; }
        public int PaymentMethodsId { get; set; }
        public int AddressId { get; set; }
        public int? VendorId { get; set; }
        public int CustomerId { get; set; }
        public int OrderStatusId { get; set; }
        public int CurrencyId { get; set; }
        public int countProduct { get; set; }
        public List<OrderProductDto> OrderProducts { get; set; }
    }
    public class CreateOrderDto
    {
       // public List<ProductDto> Products { get; set; }
        public int DeliveryTimeId { get; set; }
        public int AddressId { get; set; }
        public int? CouponId { get; set; }
        public decimal TotalTax { get; set; }
        public decimal OrderPrice { get; set; }
    }
    public class UpdateOderDto : CreateOrderDto
    {
        public int Id { get; set; }
    }
    public class OrderProductDto
    {
        public int Qty { get; set; }
        public decimal FinalPrice { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        //public List<OrderAttributProduct> OrderAttributesProducts { get; set; }
    }
    public class HistoryOrderDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public int pcount { get; set; }
        public int CustomerId { get; set; }
        public int OrderStatusId { get; set; }
        public string StatusName { get; set; }
        public string Symbol { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Quantity { get; set; }
        public decimal OrderPrice { get; set; }
        public string payUrl { get; set; }

    }
}
