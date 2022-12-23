using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entities.RequestFeatures;

namespace Repository
{
    public class ContactRepository : RepositoryBase<Contact>, IContactRepository
    {
        public ContactRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<Contact> GetContactById(int id, bool trackChanges)
         => await FindByCondition(c => c.Id.Equals(id), trackChanges).SingleOrDefaultAsync();
        public async Task<List<Contact>> GetContacts(int rows, int pageId = 1)
        {
            int SkipCount = rows * (pageId - 1);
           return await FindAll(false).OrderByDescending(x => x.CreatedAt).Skip(SkipCount).Take(rows).ToListAsync();
        }
        public int GetCountContacts() => FindAll(false).Count();
        public void CreateContact(Contact contact) => Create(contact);
        public void DeleteContact(Contact contact) => Delete(contact);
    }
}
