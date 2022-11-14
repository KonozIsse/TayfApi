namespace Entities.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Device : BaseEntity
    {
        [StringLength(250)]
        public string DeviceType { get; set; }
        [StringLength(250)]
        public string Location { get; set; }
        [StringLength(191)]
        public string DeviceModel { get; set; }
        public string OperatingSystem { get; set; }
        public short IsNotify { get; set; }
        public string DeviceToken { get; set; }
        public string FcmToken { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
