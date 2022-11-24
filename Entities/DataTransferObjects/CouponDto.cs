using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models.Enums;

namespace Entities.DataTransferObjects
{
    public class CouponDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string CouponName { get; set; }
        public string Description { get; set; }
        public string CouponCode { get; set; }
        public string DiscountType { get; set; }
        public decimal CouponAmount { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? StoreId { get; set; }
        public int? AdminId { get; set; }
        public List<int> ProductIds { get; set; }
    }
    public class CreateCouponDto
    {
        public string CouponName { get; set; }
        public string Description { get; set; }
        public string CouponCode { get; set; }
        public string DiscountType { get; set; }
        public decimal CouponAmount { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public List<string> Products { get; set; }
    }
    public class UpdateCouponDto: CreateCouponDto
    {
    }
}
