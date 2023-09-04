using Entities;
using Entities.Models;
using Entities.Models.Enums;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUserRepository
    {
        Task<User> GetUserId(int userId, bool trackChanges);
        Task<User> GetActiveUserId(int userId, bool trackChanges);
        Task<User> GetTypeUserId(int userId, UserType type, bool trackChanges);
        Task<List<User>> GetCustomers(bool trackChanges);
        Task<List<User>> GetVendorTotal(string search, bool trackChanges);
        Task<List<User>> GetAllCustomers(string search, string filter, bool trackChanges);
        Task<User> VerifiedCodeUser(int id, int code, bool trackChanges);
        Task<User> GetCustomerEmail(string email, bool trackChanges);
        Task<User> GetActiveCustomerId(int id, bool trackChanges);
        void DeleteUser(User user);
        // Admin --------------------------------------------------
        Task<IEnumerable<User>> GetAdminsStors(bool trackChanges);
        // Store --------------------------------------------------
        Task<IEnumerable<User>> GetSearchStores(string search);
        Task<IEnumerable<User>> GetAllStores(bool trackChanges);
        Task<User> GetStoreId(int id);
        Task<IEnumerable<User>> GetStoresImage(int imgId, bool trackChanges);
     }

}
