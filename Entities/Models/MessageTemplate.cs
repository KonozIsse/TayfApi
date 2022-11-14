namespace Entities.Models
{
    public class MessageTemplate : BaseEntity
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
