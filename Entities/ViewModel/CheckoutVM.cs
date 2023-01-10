using Entities.DataTransferObjects;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModel
{
    public class CheckoutVM
    {
        public List<CartVM> Cart { get; set; }
        public List<DeliveryTime> Times { get; set; }
        public List<PaymentMethods> Payment { get; set; }
        public List<CountryDto> Countries { get; set; }
        public decimal Tax { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Coupon { get; set; }
        public decimal Total { get; set; }
        public string DisCount { get; set; }
        public AddressDto Address { get; set; }
    }
}
