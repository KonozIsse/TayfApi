using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Models.Enums;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<User> GetUserId (int userId, bool trackChanges)
         => await FindByCondition(c => c.Id.Equals(userId), trackChanges).SingleOrDefaultAsync();
        public async Task<User> GetActiveUserId(int userId, bool trackChanges)
         => await FindByCondition(c => c.Id.Equals(userId) && c.Status == Status.Active, trackChanges).SingleOrDefaultAsync();
        public async Task<User> GetTypeUserId (int userId, UserType type,  bool trackChanges)
        => await FindByCondition(c => c.Id == userId && c.UserType == type, trackChanges).SingleOrDefaultAsync();
        public async Task<List<User>> GetCustomers( bool trackChanges)
        => await FindByCondition(c => c.UserType == UserType.Customer, trackChanges).OrderByDescending(c => c.CreatedAt)
            .Include(c => c.DefaultAddress).ToListAsync(); 
        public async Task<List<User>> GetAllCustomers(string search, string filter,bool trackChanges)
        {
            var customers = FindByCondition(c => c.UserType == UserType.Customer, trackChanges);
            if (!string.IsNullOrEmpty(search))
            {
                if(filter == "0")
                {
                    customers = customers.Where(c => c.FirstName.Contains(search));
                }
                else if(filter == "1")
                {
                    customers = customers.Where(c => c.Email.Contains(search));
                }
                else if (filter == "2")
                {
                    customers = customers.Where(c => c.PhoneNumber.Contains(search));
                }
                else
                {
                    customers = customers.Where(c => c.FirstName.Contains(search) || c.Email.Contains(search)
                   || c.PhoneNumber.Contains(search));
                }
            }
            return await customers.OrderByDescending(c => c.CreatedAt).Include(c => c.DefaultAddress).ToListAsync();
        }
          public async Task<User> VerifiedCodeUser(int id, int code ,bool trackChanges)
         => await FindByCondition(c => c.Id.Equals(id) && c.VerifiedCode == code, trackChanges).SingleOrDefaultAsync();
        public void DeleteUser(User user) => Delete(user);
        public async Task<User> GetCustomerEmail(string email,bool trackChanges)
        => await FindByCondition(c => c.Email == email && c.UserType == UserType.Customer && c.Status == Status.Active, trackChanges).SingleOrDefaultAsync();
        public async Task<User> GetActiveCustomerId(int id, bool trackChanges)
        => await FindByCondition(c => c.Id == id && c.UserType == UserType.Customer && c.Status == Status.Active, trackChanges).SingleOrDefaultAsync();
        // Admin 1-------------------------------------------------
        public async Task<IEnumerable<User>> GetAdminsStors (bool trackChanges)
        {
           return await FindByCondition(c => c.UserType != UserType.Customer, trackChanges).Include(c => c.Role).ToListAsync();
        }
        // Store 3--------------------------------------------------
        public async Task<List<User>> GetVendorTotal(string search, bool trackChanges)
        {
            var result = FindByCondition(c => c.UserType == UserType.Store, trackChanges);
            if (!string.IsNullOrEmpty(search))
            {
                result = result.Where(c => c.FirstName.Contains(search) || c.Email.Contains(search) || c.PhoneNumber.Contains(search));
            }
           return await result.ToListAsync();
        }
        public async Task<IEnumerable<User>> GetSearchStores(string search)
        {
            var stores = FindByCondition(c => c.UserType == UserType.Store, false);
            if (!string.IsNullOrEmpty(search))
            {
                stores = stores.Where(c => c.FirstName.Contains(search));
            }
            return await stores.Include(c => c.Products).Include(c => c.Addresses).ToListAsync();
        }
        public async Task<IEnumerable<User>> GetStoresImage(int imgId, bool trackChanges)
        => await FindByCondition(c => c.ImageId == imgId && c.UserType == UserType.Store , trackChanges).ToListAsync();

        public async Task<IEnumerable<User>> GetAllStores (bool trackChanges)
        => await FindByCondition(c => c.UserType == UserType.Store && c.Status == Status.Active, trackChanges)
            .Include(c=>c.Addresses).Include(c=>c.StoreOrders).OrderByDescending(c=>c.CreatedAt).ToListAsync();
        public async Task<User> GetStoreId(int id)
       => await FindByCondition(c =>c.Id == id && c.UserType == UserType.Store && c.Status == Status.Active, false)
            .Include(c=>c.Products).Include(c=>c.Addresses).FirstOrDefaultAsync();
       

    }
}
