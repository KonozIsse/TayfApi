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
    public class ZoneDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string ZoneName { get; set; }
        public string ZoneCode { get; set; }
        public int CountryId { get; set; }
    }
    public class CreateZoneDto
    {
        public string ZoneName { get; set; }
        public string ZoneCode { get; set; }
        public int CountryId { get; set; }
    }
}
