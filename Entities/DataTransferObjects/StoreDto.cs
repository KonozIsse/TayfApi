using Entities.Models.Enums;
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
    public class StoreDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public int? ImageId { get; set; }
        public string Avater { get; set; }
        public int? DefaultAddressId { get; set; }
        public List<AddressDto> Addresses { get; set; }
        public List<ProductsStoreDto> ProductsStores { get; set; }
       // public List<OrderDto> Orders { get; set; }

    }
    public class CreateStoreDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public string Url { get; set; }
        public int? ImageId { get; set; }
        public int? CountryId { get; set; }
    }
    public class UpdateStoreDto : CreateStoreDto
    {
    }
    public class ProductsStoreDto
    {
        public int VendorId { get; set; }
        public int ProductId { get; set; }
    }
    public class CreateCustomerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public string Avater { get; set; }
        public string Password { get; set; }
        public string Lang { get; set; }
        public TypeRegister TypeRegister { get; set; }
        public bool? IsSubscribe { get; set; }
        public string SocialImage { get; set; }
        public string SocialId { get; set; }
        public int? DefaultAddressId { get; set; }
        public int? CountryId { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Avater { get; set; }
        public string Password { get; set; }
        public Status Status { get; set; }
        public string Lang { get; set; }
        public TypeRegister TypeRegister { get; set; }
        public bool? IsMobileVerified { get; set; }
        public int? CodeMobileCountry { get; set; }
        public int? CountryId { get; set; }
        public bool? IsSubscribe { get; set; }
        public int? VerifiedCode { get; set; }
        public int? ResetPasswordCode { get; set; }
        public string Url { get; set; }
        public string SocialImage { get; set; }
        public string SocialId { get; set; }
        public int? DefaultAddressId { get; set; }
    }
    
    public class UpdateUserDto : CreateCustomerDto
    {
        public int? VerifiedCode { get; set; }
    }

    public class ResetPasswordDto
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }
    }
    public class UserForAuthenticationDto
    {
        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password name is required")]
        public string Password { get; set; }
    }
}
