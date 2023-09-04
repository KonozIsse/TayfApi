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
        public async Task<List<Contact>> GetContacts(string search , string filter)
        {
            var result = FindAll(false);
           
            if (!string.IsNullOrEmpty(search))
            {
                result = result.Where(c => c.Name.Contains(search) || c.Email.Contains(search));
            }
            if (filter == "0")
            {
                result = result.Where(c => c.Name.Contains(search));
            }
            if (filter == "1")
            {
                result = result.Where(c => c.Email.Contains(search));
            }
            return await result.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }
        public void CreateContact(Contact contact) => Create(contact);
        public void DeleteContact(Contact contact) => Delete(contact);
    }
}
