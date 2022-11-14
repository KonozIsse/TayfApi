using Entities.Models;
using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class SettingDto
    {
        public string Key { get; set; }
        public Status IsStatus { get; set; }
        public string Value { get; set; }
        public int? VendorId { get; set; }
        public int? AdminId { get; set; }
    }
    public class UpdateSettingDto
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
