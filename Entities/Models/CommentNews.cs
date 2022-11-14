namespace Entities.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    public partial class CommentNews :  BaseEntity
    {
        public string Text { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }
        [ForeignKey(nameof(News))]
        public int NewsId { get; set; }
        public News News { get; set; }
    }
}
