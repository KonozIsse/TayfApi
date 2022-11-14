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
        Task<IEnumerable<User>> GetCustomers(bool trackChanges);
        Task<User> VerifiedCodeUser(int id, int code, bool trackChanges);
        Task<User> GetSocialRegister(string socialId);
        Task<User> GetCustomerEmail(string email);
        Task<User> GetNormalRegisterUser(string email);
        Task<User> GetFacebookRegisterUser(string socialId);
        Task<User> GetGoogleRegisterUser(string socialId);
        Task<User> GetAppleRegisterUser(string socialId);
        Task<User> getLoginUser(string email, string password);
        bool CheckDefault(int userId, int defaultAddressId);
        Task<User> GetUserDefaultAddress(int userId, int defaultAddressId);
        Task<User> CheckUserPass(int user, string password);
        void AddUser(User user);
        void DeleteUser(User user);
        // Admin --------------------------------------------------
        Task<IEnumerable<User>> GetSuperAdmin(bool trackChanges);
        Task<User> GetAdminAndStoreEmail(string email);
        // Store --------------------------------------------------
        Task<IEnumerable<User>> GetStores(bool trackChanges);
        Task<IEnumerable<User>> GetAllStores(bool trackChanges);
        Task<User> GetStoreId(int id);
        Task<IEnumerable<User>> Get10Stores();
        int GetStoreCount() ;
        Task<PagedList<User>> GetAllContacts(PostsParameters postsParameters, bool trackChanges);
        
     }

}
