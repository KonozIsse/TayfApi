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
    public class CustomerProductDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal? FinalPrice { get; set; }
        public string DateAdded { get; set; }
        public int? CustomerId { get; set; }
        public int? ProductId { get; set; }
        public List<CustomerAttributesProductDto> CustomerAttributesProducts { get; set; }
    }
    public class CreateCustomerProductDto
    {
        public int Quantity { get; set; }
        public decimal? FinalPrice { get; set; }
        public List<CustomerAttributesProductDto> CustomerAttributesProducts { get; set; }
    }
    public class UpdateCustomerProductDto : CreateCustomerProductDto
    {
        public DateTime? UpdatedAt { get; set; }
    }
    public class CustomerAttributesProductDto
    {
        public int? CustomerProductId { get; set; }
        public int? AttributesProductId { get; set; }
    }

}
