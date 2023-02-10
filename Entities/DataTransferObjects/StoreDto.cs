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
        public string Status { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public string AdressInfo { get; set; }
        public string CreatedAt { get; set; }
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
        public int ImageId { get; set; }
        public string AdressInfo { get; set; }
    }
    public class UpdateStoreDto 
    {
        public int Id { get; set; }
        public string NameStore { get; set; }
        public string PhoneNumber { get; set; }
        public Status Status { get; set; }
        public string Url { get; set; }
        public int ImageId { get; set; }
        public string AdressInfo { get; set; }
    }
    public class AdminDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public string Status { get; set; }
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
