using Entities.Models;
using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
     public interface INotificationActionRepository
    {
        Task<NotificationAction> GetNotificationActionByKey(NotificationKey NewGroupPost);
    }
}
