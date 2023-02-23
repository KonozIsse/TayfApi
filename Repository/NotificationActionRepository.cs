using Contracts;
using Entities;
using Entities.Models;
using Entities.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class NotificationActionRepository: RepositoryBase<NotificationAction>, INotificationActionRepository
    {
        public NotificationActionRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        public async Task<NotificationAction> GetNotificationActionByKey(NotificationKey notificationKey)
        => await FindByCondition(c => c.NotificationKey.Equals(notificationKey), false).SingleOrDefaultAsync();
    }
}
