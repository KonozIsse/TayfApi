namespace Entities.Models
{
    using System;
    using Entities.Models.Enums;
    public class BaseEntity
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; } = Status.NotActive;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
