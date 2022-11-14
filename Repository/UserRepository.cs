using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Models.Enums;
using System.Threading.Tasks;
using Entities.RequestFeatures;

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
        public async Task<IEnumerable<User>> GetCustomers (bool trackChanges)
         => await FindByCondition(c => c.RoleId == 2 , trackChanges).ToListAsync();
        public bool CheckDefault(int userId ,int defaultAddressId)
        => FindByCondition(t => t.Id == userId && t.DefaultAddressId == defaultAddressId, false).Count() > 0;
        public async Task<User> GetUserDefaultAddress(int userId, int defaultAddressId)
        => await FindByCondition(c => c.Id.Equals(userId) && c.DefaultAddressId == defaultAddressId, false).SingleOrDefaultAsync();
        public async Task<User> VerifiedCodeUser(int id, int code ,bool trackChanges)
         => await FindByCondition(c => c.Id.Equals(id) && c.VerifiedCode == code, trackChanges).SingleOrDefaultAsync();
        public async Task<User> GetCustomerEmail(string email)
         => await FindByCondition(c => c.Email == email && c.RoleId == 2 && c.Status == Status.Active ,false).SingleOrDefaultAsync();
       
        public async Task<User> GetSocialRegister(string socialId)
          => await FindByCondition(c => c.Status == Status.Active && c.SocialId == socialId, false).SingleOrDefaultAsync();
        public async Task<User> GetNormalRegisterUser(string email)
        => await FindByCondition(c => c.Status == Status.Active && c.Email == email && c.TypeRegister == TypeRegister.Normal ,false).SingleOrDefaultAsync();
        public async Task<User> GetFacebookRegisterUser(string socialId)
        => await FindByCondition(c => c.Status == Status.Active && c.SocialId == socialId && c.TypeRegister == TypeRegister.Facebook, false).SingleOrDefaultAsync();
        public async Task<User> GetGoogleRegisterUser(string socialId)
         => await FindByCondition(c => c.Status == Status.Active && c.SocialId == socialId && c.TypeRegister == TypeRegister.Google, false).SingleOrDefaultAsync();
        public async Task<User> GetAppleRegisterUser(string socialId)
          => await FindByCondition(c => c.Status == Status.Active && c.SocialId == socialId && c.TypeRegister == TypeRegister.Apple, false).SingleOrDefaultAsync();
        public async Task<User> getLoginUser(string email, string password)
         => await FindByCondition(c => c.Status == Status.Active && c.Email == email && c.Password == password && c.TypeRegister == TypeRegister.Normal, false).SingleOrDefaultAsync();
        public async Task<User> CheckUserPass(int user, string password)
        => await FindByCondition(c => c.Id == user && c.Password == password && c.Status == Status.Active, false).SingleOrDefaultAsync();
        public void AddUser(User user) => Create(user);
        public void DeleteUser(User user) => Delete(user);
        
        // Admin 1--------------------------------------------------
        public async Task<IEnumerable<User>> GetSuperAdmin(bool trackChanges)
         => await FindByCondition(c => c.RoleId == 1, trackChanges).ToListAsync();
        public async Task<User> GetAdminAndStoreEmail(string email)
         => await FindByCondition(c => c.Email == email && c.RoleId != 2 && c.Status == Status.Active, false).SingleOrDefaultAsync();
        // Store 3--------------------------------------------------
        public async Task<IEnumerable<User>> GetStores(bool trackChanges)
        => await FindByCondition(c => c.RoleId == 3, trackChanges).ToListAsync();
        public async Task<IEnumerable<User>> GetAllStores (bool trackChanges)
        => await FindByCondition(c => c.RoleId == 3 && c.Status == Status.Active, trackChanges).ToListAsync();
        public async Task<User> GetStoreId (int id)
       => await FindByCondition(c =>c.Id == id && c.RoleId == 3 && c.Status == Status.Active, false).FirstOrDefaultAsync();
        public async Task<IEnumerable<User>> Get10Stores()
        => await FindByCondition(c => c.RoleId == 3 && c.Status == Status.Active, false).OrderBy(r => r.Id).Take(10).ToListAsync();
        public int GetStoreCount() => FindByCondition(c => c.RoleId == 3 && c.Status == Status.Active, false).Count();
        public async Task<PagedList<User>> GetAllContacts(PostsParameters postsParameters, bool trackChanges)
        {
            var user = await FindByCondition(c => c.RoleId == 3 && c.Status == Status.Active, trackChanges).OrderByDescending(c => c.CreatedAt).ToListAsync();
            return PagedList<User>.ToPagedList(user, postsParameters.PageNumber, postsParameters.PageSize);
        }

    }
}
