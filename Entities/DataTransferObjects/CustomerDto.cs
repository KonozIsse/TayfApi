using Entities.Models.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
   
    public class CustomerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Avater { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Total { get; set; }
    }
    
    public class CreateCustomerDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public TypeRegister TypeRegister { get; set; }
        public bool IsSubscribe { get; set; }
        [Required]
        public int CountryId { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public int Agree { get; set; }

    }
    public class CreateCustomerCPDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public int CountryId { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public Status Status { get; set; }
    }
    public class UpdateCustomerDto : CreateCustomerCPDto
    {
        public int Id { get; set; }
        public string ConfirmedPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
