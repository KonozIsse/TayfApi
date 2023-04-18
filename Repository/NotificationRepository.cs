using Contracts;
using Entities;
using Entities.Models;
using Entities.Models.Enums;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
    {
        public NotificationRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<IEnumerable<Notification>> GetNotifications(bool trackChanges)
        => await FindAll(trackChanges).OrderBy(c=>c.CreatedAt).ToListAsync();
        public async Task<List<Notification>> GetNotificationsToUserId (int userId , bool trackChanges)
        => await FindByCondition(c=> c.UserId == userId , trackChanges).ToListAsync();
         public async Task<List<Notification>> GetAllNotifications(bool trackChanges)
        => await FindAll(trackChanges).Include(c=>c.User).Include(c=>c.NotificationAction)
                .OrderByDescending(c => c.Id).ToListAsync();
        public async Task<Notification> FindNotificationId (int id, bool trackChanges)
        => await FindByCondition(c=>c.Id == id, trackChanges).FirstOrDefaultAsync();
        public void CreateNotification(Notification notification) => Create(notification);
        public void DeleteNotification(Notification notification) => Delete(notification);
    }
}
