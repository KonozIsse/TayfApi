namespace Entities.Models
{
    public class Contact : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public bool? IsRead { get; set; }
    }
}
