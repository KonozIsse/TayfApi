using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models.Enums;

namespace Entities.DataTransferObjects
{
    public class DeviceDto
    {
        public int Id { get; set; }
        public Status IsStatus { get; set; }
        public string DeviceType { get; set; }
        public string Location { get; set; }
        public string DeviceModel { get; set; }
        public string OperatingSystem { get; set; }
        public int UserId { get; set; }
    } 
    public class CreateDeviceDto
    {
        public string DeviceType { get; set; }
        public string Location { get; set; }
        public string DeviceModel { get; set; }
        public string OperatingSystem { get; set; }
        public string DeviceToken { get; set; }
        public int UserId { get; set; }
    } 
    public class UpdateDeviceDto : CreateDeviceDto
    {
    }
}
