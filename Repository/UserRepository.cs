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
        public async Task<User> GetCustomerId (int customerId, bool trackChanges)
        => await FindByCondition(c => c.Id == customerId && c.RoleId == 2, trackChanges).SingleOrDefaultAsync();
        public async Task<List<User>> GetCustomers( bool trackChanges)
        => await FindByCondition(c => c.RoleId == 2, trackChanges).OrderByDescending(c => c.CreatedAt)
            .Include(c => c.DefaultAddress).ToListAsync(); 
        public async Task<List<User>> GetVendorTotal (string search, bool trackChanges)
        => await FindByCondition(c => c.RoleId == 3 && c.FirstName.Contains(search) || c.Email.Contains(search)|| c.PhoneNumber.Contains(search), trackChanges).ToListAsync();
        public async Task<List<User>> GetAllCustomers(string search, bool trackChanges)
        {
            var customers = FindByCondition(c => c.RoleId == 2, trackChanges);
            if (!string.IsNullOrEmpty(search))
            {
                customers.Where(c => c.FirstName.Contains(search) || c.Email.Contains(search)
                || c.PhoneNumber.Contains(search)|| c.DefaultAddress.AddressTitle.Contains(search));
            }
            return await customers.OrderByDescending(c => c.CreatedAt).Include(c => c.DefaultAddress).ToListAsync();
        }
        public async Task<User> GetUserDefaultAddress(int userId, int defaultAddressId)
        => await FindByCondition(c => c.Id.Equals(userId) && c.DefaultAddressId == defaultAddressId, false).SingleOrDefaultAsync();
        public async Task<User> VerifiedCodeUser(int id, int code ,bool trackChanges)
         => await FindByCondition(c => c.Id.Equals(id) && c.VerifiedCode == code, trackChanges).SingleOrDefaultAsync();
        public void DeleteUser(User user) => Delete(user);
        public async Task<User> GetCustomerEmail(string email,bool trackChanges)
        => await FindByCondition(c => c.Email == email && c.RoleId == 2 && c.Status == Status.Active, trackChanges).SingleOrDefaultAsync();
        public async Task<User> GetActiveCustomerId(int id, bool trackChanges)
        => await FindByCondition(c => c.Id == id && c.RoleId == 2 && c.Status == Status.Active, trackChanges).SingleOrDefaultAsync();
        // Admin 1-------------------------------------------------
        public async Task<IEnumerable<User>> GetAdminsStors (bool trackChanges)
         => await FindByCondition(c => c.RoleId != 2, trackChanges).Include(c=>c.Role).ToListAsync();
        public async Task<User> GetAdminAndStoreEmail(string email)
         => await FindByCondition(c => c.Email == email && c.RoleId != 2 && c.Status == Status.Active, false).SingleOrDefaultAsync();
        // Store 3--------------------------------------------------
        public async Task<User> GetStore(int storeId, bool trackChanges)
        => await FindByCondition(c => c.Id == storeId && c.RoleId == 3 && c.Status == Status.Active, trackChanges).SingleOrDefaultAsync();
        public async Task<IEnumerable<User>> GetSearchStores(string search)
        {
            var stores = FindByCondition(c => c.RoleId == 3, false);
            if (string.IsNullOrEmpty(search))
            {
                stores.Where(c => c.FirstName.Contains(search));
            }
            return await stores.Include(c => c.Products).Include(c => c.Addresses).ToListAsync();
        }
        public async Task<IEnumerable<User>> GetStoresImage(int imgId, bool trackChanges)
        => await FindByCondition(c => c.ImageId == imgId && c.RoleId == 3 , trackChanges).ToListAsync();

        public async Task<IEnumerable<User>> GetAllStores (bool trackChanges)
        => await FindByCondition(c => c.RoleId == 3 && c.Status == Status.Active, trackChanges)
            .Include(c=>c.Addresses).Include(c=>c.StoreOrders).OrderByDescending(c=>c.CreatedAt).ToListAsync();
        public async Task<User> GetStoreId(int id)
       => await FindByCondition(c =>c.Id == id && c.RoleId == 3 && c.Status == Status.Active, false)
            .Include(c=>c.Products).Include(c=>c.Addresses).FirstOrDefaultAsync();
        public async Task<IEnumerable<User>> Get10Stores()
        => await FindByCondition(c => c.RoleId == 3 && c.Status == Status.Active, false)
            .OrderByDescending(r => r.CreatedAt).Take(10).ToListAsync();
    

    }
}
