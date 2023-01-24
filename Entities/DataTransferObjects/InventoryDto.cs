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
        public int Id { get; set; }
        public int Stock { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } 
        public string AddedBy { get; set; } 
        public string StockType { get; set; }
        public string PurchaseCode { get; set; }
        public string CreatedAt { get; set; } 
        public string UpdateAt { get; set; }
        public int Total { get; set; }
    } 
    public class CreateInventoryDto
    {
        public int Stock { get; set; }
        public int ProductId { get; set; }
        public decimal TotalPurchasedPrice { get; set; }
        public string PurchaseCode { get; set; }
        public int? AttributesProductId { get; set; }
    }
   
}
