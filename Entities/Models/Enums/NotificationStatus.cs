using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Enums
{
   [EnumBindResource(typeof(ResourcesLib.EnumResources.NotificationStatus))]
    public enum NotificationStatus
    {
        New = 1,
        Notifified = 2,
        Red = 3
    }
}
