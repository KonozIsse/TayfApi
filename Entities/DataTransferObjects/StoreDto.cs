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
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public Status Status { get; set; }
        [DataType(DataType.Url)]
        public string Url { get; set; }
        [Required]
        public int ImageId { get; set; }
        [Required]
        public string AdressInfo { get; set; }
    }
    public class UpdateStoreDto 
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public Status Status { get; set; }
        public string Url { get; set; }
        [Required]
        public int ImageId { get; set; }
        [Required]
        public string AdressInfo { get; set; }
    }
    public class AdminDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public UserType UserType { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public string Status { get; set; }
    }
    public class CreateAdminDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public Status Status { get; set; }
        [Required]
        public int CountryId { get; set; }
        [Required]
        public int RoleId { get; set; }
    }
    public class UpdateAdminDto : CreateAdminDto
    {
        public int Id { get; set; }
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
    }
  
}

