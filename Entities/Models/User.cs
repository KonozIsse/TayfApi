namespace Entities.Models
{
    using Entities.Models.Enums;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class User : IdentityUser<int>
    { 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [NotMapped]
        public string FullName { get { return $"{FirstName} {LastName}"; } }
        public string Avater { get; set; }
        public Status Status { get; set; }
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public string Lang { get; set; }
        public TypeRegister TypeRegister { get; set; }
        public bool? IsMobileVerified { get; set; }
        public int? CodeMobileCountry { get; set; }
        public bool? IsSubscribe { get; set; }
        public int? VerifiedCode { get; set; }
        public int? ResetPasswordCode { get; set; }
        public string Url { get; set; }
        public string SocialImage { get; set; }
        public string SocialId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        //[ForeignKey(nameof(Image))]
        //public int? ImageId { get; set; }
        //public Image Image { get; set; }

        [ForeignKey(nameof(DefaultAddress))]
        public int? DefaultAddressId { get; set; }
        public Address DefaultAddress { get; set; }

        [ForeignKey(nameof(Country))]
        public int? CountryId { get; set; }
        public Country Country { get; set; }

        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public List<Device> Devices { get; set; }
        public List<Address> Addresses { get; set; }
        public List<Product> Products { get; set; }
        public List<Order> StoreOrders { get; set; }
        public List<Order> CustomerOrders { get; set; }
    }
}
