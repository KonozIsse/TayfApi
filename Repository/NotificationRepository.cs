using Contracts;
using Entities;
using Entities.Models;
using Entities.Models.Enum;
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
        public async Task<IEnumerable<Notification>> GetNewNotifications(bool trackChanges)
        => await FindAll(trackChanges).Where(c => c.Status == NotificationStatus.New)
            .Include(u=>u.User).OrderBy(c => c.CreatedAt).ToListAsync();
        public async Task<List<Notification>> GetNotificationsPage(int PageId, int rows)
        {
            int SkipCount = rows * (PageId - 1);
            return await FindAll(false).Skip(SkipCount).Take(rows).OrderByDescending(c => c.Id).ToListAsync();
        }
        public async Task<Notification> FindNotificationId (int id, bool trackChanges)
        => await FindByCondition(c=>c.Id == id, trackChanges).FirstOrDefaultAsync();
        public void CreateNotification(Notification notification) => Create(notification);
        public void DeleteNotification(Notification notification) => Delete(notification);
        public int GetNotificationsCount() => FindAll(false).Count();
        public int GetNotificationCountUserNotRead(int userId) => FindByCondition(c => c.UserId == userId && c.IsRead == false,false ).Count();
        public int GetNotificationCountUser(int userId) => FindByCondition(c => c.UserId == userId , false).Count();
    }
}
