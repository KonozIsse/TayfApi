using Entities.Models.Enums;
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
        public string Url { get; set; }
        public string Icon { get; set; }
        public int ParentId { get; set; }
        public int OrderId { get; set; }
        public bool? IsVendorLink { get; set; }
        public NavSubmenu NavSubmenu { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
