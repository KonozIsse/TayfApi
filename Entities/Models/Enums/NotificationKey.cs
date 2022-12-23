using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Enum
{
  //[EnumBindResource(typeof(ResourcesLib.EnumResources.NotificationKey))]
    public enum NotificationKey
    {
        ShippedOrder = 1,
        CompleteOrder,
        RejectOrder,
        ReceiveOrder,
        CancelOrder
    }
}