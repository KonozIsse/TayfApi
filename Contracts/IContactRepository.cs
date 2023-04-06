using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IContactRepository
    {
        Task<Contact> GetContactById(int id, bool trackChanges);
        int GetCountContacts();
        Task<List<Contact>> GetContacts(string search, string filter);
        void CreateContact(Contact contact);
        void DeleteContact(Contact contact);
    }
}
