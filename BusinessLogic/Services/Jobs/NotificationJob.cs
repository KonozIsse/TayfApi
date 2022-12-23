using Contracts;
using Entities.Models.CorePushModels;
using Entities.Models.Enum;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Language;
using BusinessLogic.Services;

namespace BusinessLogic.Services.Jobs
{
    public class NotificationJob : IJob
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly INotificationService _notificationService;

        public NotificationJob(IRepositoryManager repositoryManager, INotificationService notificationService)
        {
            _repositoryManager = repositoryManager;
            _notificationService = notificationService;
        }

        async Task IJob.Execute(IJobExecutionContext context)
        {
            var notify = await _repositoryManager.Notification.GetNotifications(true);
            foreach (var item in notify)
            {
                var token = await _repositoryManager.Device.GetDeviceUserId(item.UserId);
                var result = await _notificationService.SendNotification(new NotificationModel
                {
                    Body = item.Body,
                    DeviceId = token.FcmToken,
                    IsAndroiodDevice = true,
                    Title = item.Subject
                });
                if (result.IsSuccess)
                {
                    item.Status = NotificationStatus.Notifified;
                }
            }
            await _repositoryManager.SaveAsync();
        }
    }
}
