using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class DeliveryTime : BaseEntity
    {
        public string Time { get; set; }
    }
}
