using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class InventoryDto
    {
        public string StockType { get; set; }
        public int Stock { get; set; }
        public decimal TotalPurchasedPrice { get; set; }
        public string PurchaseCode { get; set; }
        public int AddedDate { get; set; }
        public int ProductId { get; set; }
        public int? AttributesProductId { get; set; }
        public int? VendorId { get; set; }
        public int? AdminId { get; set; }
    } 
    public class CreateInventoryDto
    {
        public int Stock { get; set; }
        public int ProductId { get; set; }
        public int? AttributesProductId { get; set; }
    }
    public class UpdateInventoryDto
    {
        public int Stock { get; set; }
        public decimal TotalPurchasedPrice { get; set; }
        public string PurchaseCode { get; set; }
        public int ProductId { get; set; }
        public int? AttributesProductId { get; set; }
    }
}
