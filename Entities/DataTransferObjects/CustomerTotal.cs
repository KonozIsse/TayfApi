using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class CustomerTotal
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int Total { get; set; }
    }
    public class CreateCustomerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public Status Status { get; set; }
        public int? CountryId { get; set; }
    } 
    public class CreateCustomer2Dto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Avater { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public TypeRegister TypeRegister { get; set; }
        public bool? IsSubscribe { get; set; }
        public string SocialImage { get; set; }
        public string SocialId { get; set; }
        public int? CountryId { get; set; }
    }
    public class UpdateCustomerDto : CreateCustomerDto
    {
        public int Id { get; set; }
    }
}
