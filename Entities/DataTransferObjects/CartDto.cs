using Entities.Models;
using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class CartDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Qty { get; set; }
        public Status IsStatus { get; set; }
        public decimal FinalPrice { get; set; }
        public int CustomerId { get; set; }
        public List<CartAttributeProductDto> CartAttributeProducts { get; set; }
        public object StoreGrouped { get; set; }
    }
    public class CreateCartDto
    {
        public int ProductId { get; set; }
        public int Qty { get; set; }
        public List<CartAttributeProductDto> CartAttributeProducts { get; set; }
    }
    public class UpdateCartDto: CreateCartDto
    {
        public int Id { get; set; }
    }
    public class CartAttributeProductDto
    {
        public int AttributesProductId { get; set; }
    }
}
