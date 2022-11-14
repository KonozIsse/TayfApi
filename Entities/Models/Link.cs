using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Link : BaseEntity
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public int ParentLinkId { get; set; }
        public int OrderedId { get; set; }
        public bool Show { get; set; }
        public bool? IsVendorLink { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
