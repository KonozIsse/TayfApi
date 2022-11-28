namespace Entities.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Order : BaseEntity
    {
        public decimal TotalTax { get; set; }
        public DateTime? DatePurchased { get; set; }
        public DateTime? OrderDateFinished { get; set; }
        public decimal OrderPrice { get; set; }
        public string TransactionId { get; set; }
        public string HashedCtpAndPayment { get; set; }
        public string Notes { get; set; }
        [ForeignKey(nameof(ShippingMethod))]
        public int? ShippingMethodId { get; set; }
        public ShippingMethods ShippingMethod { get; set; }
        public short IsSeen { get; set; }
        [ForeignKey(nameof(DeliveryTime))]
        public int DeliveryTimeId { get; set; }
        public DeliveryTime DeliveryTime { get; set; }
        public string CodeCoupon { get; set; }
        public Coupon Coupon { get; set; }

        [ForeignKey(nameof(PaymentMethods))]
        public int PaymentMethodsId { get; set; }
        public PaymentMethods PaymentMethods { get; set; }

        [ForeignKey(nameof(Address))]
        public int AddressId { get; set; }
        public Address Address { get; set; }

        [ForeignKey(nameof(Store))]
        public int StoreId { get; set; }
        public User Store { get; set; }

        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public User Customer { get; set; }

        [ForeignKey(nameof(OrderStatus))]
        public int OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }

        [ForeignKey(nameof(Currency))]
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public List<OrderProduct> OrderProducts { get; set; }
    }
}
