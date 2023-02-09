using Entities.Models.Enums;

namespace Entities.Models
{
    public class StaticPages : BaseEntity
    {
        public string Title { get; set; }
        public string TitleAr { get; set; }
        public string Description { get; set; }
        public string DescriptionAr { get; set; }
        public PageType PageType { get; set; }
    }
}
