using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models.Enums;

namespace Entities.DataTransferObjects
{
    public class AddressDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string AddressTitle { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Street { get; set; }
        public string Post_Code { get; set; }
        public string CityName { get; set; }
        public string Flat { get; set; }
        public bool IsDefault { get; set; }
        public int ZoneId { get; set; }
        public int CountryId { get; set; }
        public int UserId { get; set; }
    }
    public class CreateAddressDto
    {
        [Required]
        public string AddressTitle { get; set; }
        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string Post_Code { get; set; }
        [Required]
        public string CityName { get; set; }
        [Required]
        public string Flat { get; set; }
        public bool IsDefault { get; set; }
        [Required]
        public int ZoneId { get; set; }
        [Required]
        public int CountryId { get; set; }
    }
    public class UpdateAddressDto: CreateAddressDto
    {
        public int Id { get; set; }
    }
}
