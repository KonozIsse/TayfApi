using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetNotifications(bool trackChanges);
        Task<IEnumerable<Notification>> GetNewNotifications(bool trackChanges);
        Task<List<Notification>> GetNotificationsToUserId(int userId, bool trackChanges);
        Task<List<Notification>> GetNotificationsPage(int PageId, int rows);
        Task<Notification> FindNotificationId(int id, bool trackChanges);
        void CreateNotification(Notification notification);
        void DeleteNotification(Notification notification);
        int GetNotificationsCount();
        int GetNotificationCountUserNotRead(int userId);
        int GetNotificationCountUser(int userId);
    }
}
