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
        public async Task<IEnumerable<Address>> GetAllAddressesByCustomerId(int customerId)
         => await FindByCondition(c => c.UserId == customerId, false).OrderByDescending(x => x.Id).ToListAsync();
        public async Task<Address> GetAddressIdByCustomerId(int id ,int customerId , bool trackChanges)
          => await FindByCondition(c => c.Id == id && c.UserId == customerId, trackChanges).SingleOrDefaultAsync();
        public async Task<Address> GetAddressCustomer( int customerId)
        => await FindByCondition(c => c.UserId == customerId, false).SingleOrDefaultAsync();
        public void AddAddress(Address address) => Create(address);
        public void DeleteAddress(Address address) => Delete(address);
    }
}
