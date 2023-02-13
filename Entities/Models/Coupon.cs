namespace Entities.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Coupon : BaseEntity
    {
        public string CouponName { get; set; }
        public string Description { get; set; }
        public string CouponNameAr { get; set; }
        public string DescriptionAr { get; set; }
        [Required]
        [StringLength(191)]
        public string CouponCode { get; set; }
        [Required]
        [StringLength(100)]
        public string DiscountType { get; set; }
        public decimal CouponAmount { get; set; }
        public DateTime? ExpiryDate { get; set; }

        [ForeignKey(nameof(Store))]
        public int? StoreId { get; set; }
        public User Store { get; set; }

        [ForeignKey(nameof(Admin))]
        public int? AdminId { get; set; }
        public User Admin { get; set; }
       // public List<Product> ListProducts { get; set; }
        public string Products { get; set; } 
        //public List<int> ProductIds { get; set; }
    }
}
