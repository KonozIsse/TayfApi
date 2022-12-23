using Entities.Models.Enums;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsVendorLink { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
        public Status IsStatus { get; set; } 
    } 
    public class CreateRoleDto
    {
        public string Name { get; set; }
        public bool? IsVendorLink { get; set; }
        public Status IsStatus { get; set; }
    }
    public class UpdateRoleDto : CreateRoleDto
    {
        public int Id { get; set; }
    }
}
