namespace Entities.Models
{
    using Entities.Models.Enums;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class Role : IdentityRole<int>
    { 
        [StringLength(191)]
        public string Name { get; set; }
        public bool? IsVendorLink { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Status IsStatus { get; set; } = Status.NotActive;
        public List<User> Users { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
