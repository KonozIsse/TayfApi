using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class AddressRepository : RepositoryBase<Address>, IAddressRepository
    {
        public AddressRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<Address> GetAddress (int id, bool trackChanges)
         => await FindByCondition(c => c.Id == id, trackChanges).SingleOrDefaultAsync();
        public async Task<List<Address>> GetAllAddressesByCustomerId(int customerId)
         => await FindByCondition(c => c.UserId == customerId, false).Include(c=>c.User).Include(c=>c.Country).OrderByDescending(x => x.Id).ToListAsync();
        public async Task<Address> GetAddressIdByCustomerId(int id ,int customerId , bool trackChanges)
          => await FindByCondition(c => c.Id == id && c.UserId == customerId&& c.IsDefault == true, trackChanges).SingleOrDefaultAsync();
        public async Task<Address> GetAddressCustomer( int customerId)
        => await FindByCondition(c => c.UserId == customerId && c.IsDefault == true, false).SingleOrDefaultAsync();
        public void AddAddress(Address address) => Create(address);
        public void DeleteAddress(Address address) => Delete(address);
    }
}
