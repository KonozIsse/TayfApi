namespace Entities.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Inventory : BaseEntity
    {
        [Required]
        [StringLength(10)]
        public string StockType { get; set; }
        public int Stock { get; set; }
        public decimal TotalPurchasedPrice { get; set; }
        [StringLength(191)]
        public string PurchaseCode { get; set; }
        public int AddedDate { get; set; } 
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [ForeignKey(nameof(AttributesProduct))]
        public int? AttributesProductId { get; set; }
        public ProductAttribut AttributesProduct { get; set; }

        [ForeignKey(nameof(Vendor))]
        public int? VendorId { get; set; }
        public User Vendor { get; set; }

        [ForeignKey(nameof(Admin))]
        public int? AdminId { get; set; }
        public User Admin { get; set; }
    }
}
