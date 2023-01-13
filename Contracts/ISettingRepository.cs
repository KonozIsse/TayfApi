using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ISettingRepository
    {
        Task<IEnumerable<Setting>> GetAllSettings(bool trackChanges);
        Task<Setting> GetSettingByValue(string key, bool trackChanges);
        Task<IEnumerable<Setting>> GetMediaSetting();
         string GetPeriod();
    } 
    public interface IMessageTemplateRepository
    {
        Task<MessageTemplate> GetVerificationEmail();
        Task<MessageTemplate> GetDefaultEmailTemplate();
        Task<IEnumerable<MessageTemplate>> GetEmailTemplatesList(bool trackChanges);
        Task<MessageTemplate> GetTemplateById(int id, bool trackChanges);
    }
    public interface IMailListRepository
    {
        Task<List<MailList>> GetMailLists();
        Task<MailList> GetMailListById(int id, bool trackChanges);
        Task<List<MailList>> GetMailListEmail(string search);
        Task<MailList> GetEmail(string email);
        void SendUserEmail(MailList sendEmail);
        void RemoveMailList(MailList email);
    }
    public interface IDeliveryTimeRepository
    {
        Task<List<DeliveryTime>> GetAllDeliveryTimes();
        Task<DeliveryTime> GetDeliveryTimeById(int id, bool trackChanges);
    }
}
