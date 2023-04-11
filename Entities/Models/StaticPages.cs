using Entities.Models.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Entities.Models
{
    public class StaticPages : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public PageType PageType { get; set; }
        [NotMapped]
        public Dictionary<string, string> Names
        {
            get { return Title == null ? null : JsonConvert.DeserializeObject<Dictionary<string, string>>(Title); }
            set { Title = JsonConvert.SerializeObject(value); }
        }

        // all translations of description
        [NotMapped]
        public Dictionary<string, string> Descriptions
        {
            get { return Description == null ? null : JsonConvert.DeserializeObject<Dictionary<string, string>>(Description); }
            set { Description = JsonConvert.SerializeObject(value); }
        }
    }
}
