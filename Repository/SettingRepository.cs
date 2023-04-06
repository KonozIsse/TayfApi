using Contracts;
using Entities.Models;
using Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Entities.Models.Enums;

namespace Repository
{
    public class SettingRepository : RepositoryBase<Setting>, ISettingRepository
    {
        public SettingRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<IEnumerable<Setting>> GetAllSettings(bool trackChanges) => await FindAll(trackChanges).ToListAsync();
        public async Task<Setting> GetSettingByValue(string key, bool trackChanges)
          => await FindByCondition(c => c.Key == key, trackChanges).FirstOrDefaultAsync();
        public async Task<IEnumerable<Setting>> GetMediaSetting()
         => await FindByCondition(x => x.Key == "Thumbnail_height" || x.Key == "Thumbnail_width" || x.Key == "Medium_height" ||
         x.Key == "Medium_width" || x.Key == "Large_height" || x.Key == "Large_width", false).ToListAsync();
        public string GetPeriod() => FindByCondition(r => r.Key == "Refund_Money_Days", false).SingleOrDefault().Value;
    }
    public class MessageTemplateRepository : RepositoryBase<MessageTemplate>, IMessageTemplateRepository
    {
        public MessageTemplateRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<MessageTemplate> GetNameTemplate(NameTemplate name)
        => await FindByCondition(c => c.NameTemplate == name, false).FirstOrDefaultAsync();
        public async Task<MessageTemplate> GetTemplateById(int id, bool trackChanges)
        => await FindByCondition(c => c.Id == id, trackChanges).FirstOrDefaultAsync();
        public async Task<IEnumerable<MessageTemplate>> GetEmailTemplatesList (bool trackChanges)
         => await FindAll(trackChanges).ToListAsync();
    }
    public class MailListRepository : RepositoryBase<MailList>, IMailListRepository
    {
        public MailListRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<MailList> GetMailListById(int id, bool trackChanges)
        => await FindByCondition(c => c.Id == id, trackChanges).FirstOrDefaultAsync(); 
        public async Task<MailList> GetEmail(string email)
        => await FindByCondition(c => c.Email == email, false).FirstOrDefaultAsync();
        public async Task<List<MailList>> GetMailListEmail(string search)
        {
            var result = FindAll(false);
            if (!string.IsNullOrWhiteSpace(search))
            {
                result = result.Where(c => c.Email.Contains(search));
            }
            return await result.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }
        public void SendUserEmail(MailList sendEmail) => Create(sendEmail);
        public void RemoveMailList(MailList email) => Delete(email);
    }
    public class DeliveryTimeRepository : RepositoryBase<DeliveryTime>, IDeliveryTimeRepository
    {
        public DeliveryTimeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<List<DeliveryTime>> GetAllDeliveryTimes()
        => await FindByCondition(c => c.IsStatus == Status.Active, false).ToListAsync();
       public async Task<DeliveryTime> GetDeliveryTimeById(int id, bool trackChanges)
        => await FindByCondition(c => c.Id == id && c.IsStatus == Status.Active, trackChanges).FirstOrDefaultAsync();
    }
}

