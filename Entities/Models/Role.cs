namespace Entities.Models
{
    using Entities.Models.Enums;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class Role : IdentityRole<int>
    { 
        public bool? IsVendorLink { get; set; }
        public TimeSlot TimeSlot { get; set; } 
        public Status IsStatus { get; set; } = Status.NotActive;
        public List<User> Users { get; set; }
        public List<Permission> Permissions { get; set; }
    }
    public class TimeSlot
    {
        public TimeSpan CreatedAt { get; set; } 
        public TimeSpan? UpdatedAt { get; set; }
    }
}
