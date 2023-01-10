using Entities;
using Entities.Models;
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
        Task<User> GetCustomerId(int customerId, bool trackChanges);
        Task<List<User>> GetCustomerTotal(string search, bool trackChanges);
        Task<List<User>> GetVendorTotal(string search, bool trackChanges);
        Task<List<User>> GetCustomers(bool trackChanges);
        Task<User> VerifiedCodeUser(int id, int code, bool trackChanges);
        Task<User> GetSocialRegister(string socialId);
        bool GetEmail(string email);
        Task<User> GetNormalRegisterUser(string email);
        Task<User> GetFacebookRegisterUser(string socialId);
        Task<User> GetGoogleRegisterUser(string socialId);
        Task<User> GetAppleRegisterUser(string socialId);
        Task<User> getLoginUser(string email, string password);
        Task<User> GetUserDefaultAddress(int userId, int defaultAddressId);
        Task<User> CheckUserPass(int user, string password);
        Task<User> GetCustomerEmail(string email, bool trackChanges);
        Task<User> GetActiveCustomerId(int id, bool trackChanges);
        void DeleteUser(User user);
        // Admin --------------------------------------------------
        Task<IEnumerable<User>> GetAdminsStors(bool trackChanges);
        Task<User> GetAdminAndStoreEmail(string email);
        // Store --------------------------------------------------
       Task<User> GetStore(int stored, bool trackChanges);
        Task<IEnumerable<User>> GetStores(bool trackChanges);
        Task<IEnumerable<User>> GetAllStores(bool trackChanges);
        Task<User> GetStoreId(int id);
        Task<IEnumerable<User>> Get10Stores();
        Task<IEnumerable<User>> GetStoreList(int PageId = 1, int rows = 10);
        int GetStoresCount();
     }

}
