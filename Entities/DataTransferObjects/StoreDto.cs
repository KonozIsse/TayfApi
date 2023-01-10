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
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Avater { get; set; }
        public string AdressInfo { get; set; }
        public DateTime CreatedAt { get; set; }
        // public object CartGrouped { get; set; }
        public decimal TotalPrice { get; set; }
        public int CountCart { get; set; }
    }
    public class CreateStoreDto
    {
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Status Status { get; set; }
        public string Url { get; set; }
        public string Lang { get; set; }
        public string Avater { get; set; }
        public string AdressInfo { get; set; }
    }
    public class UpdateStoreDto 
    {
        public int Id { get; set; }
        public string NameStore { get; set; }
        public string PhoneNumber { get; set; }
        public Status Status { get; set; }
        public string Url { get; set; }
        public string Avater { get; set; }
        public string AdressInfo { get; set; }
    }
 
    public class CreateAdminDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Status Status { get; set; }
        public int? CountryId { get; set; }
        public int RoleId { get; set; }
        public string AdressInfo { get; set; }
    }
    public class UpdateAdminDto : CreateAdminDto
    {
        public int Id { get; set; }
        public string OldPassword { get; set; }
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
        public string AdressInfo { get; set; }
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
        public string Code { get; set; }
    }
    public class UserForAuthenticationDto
    {
        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password name is required")]
        public string Password { get; set; }
    }
}
