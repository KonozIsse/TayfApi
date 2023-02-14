using Entities.Models.Enums;

namespace Entities.Models
{
    public class MessageTemplate : BaseEntity
    {
        public NameTemplate NameTemplate { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
