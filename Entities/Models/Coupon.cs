namespace Entities.Models
{
    using Entities.Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Coupon : BaseEntity
    {
        public string Description { get; set; }
        [Required]
        [StringLength(191)]
        public string CouponCode { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal CouponAmount { get; set; }
        public DateTime? ExpiryDate { get; set; }

        [ForeignKey(nameof(Store))]
        public int? StoreId { get; set; }
        public User Store { get; set; }

        [ForeignKey(nameof(Admin))]
        public int? AdminId { get; set; }
        public User Admin { get; set; }
        public List<ProductsCoupon> ProductsCoupons { get; set; }
    }
}
