namespace Entities.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    public partial class CommentNews :  BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
        [ForeignKey(nameof(News))]
        public int NewsId { get; set; }
        public News News { get; set; }
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public User Customer { get; set; }
    }
}
