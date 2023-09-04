using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entities.Models.Enums;

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
          => await FindByCondition(c => c.Id == id && c.UserId == customerId && c.IsStatus == Status.Active, trackChanges).SingleOrDefaultAsync();
        public async Task<Address> GetDefaultAddressCustomer( int customerId)
        => await FindByCondition(c => c.UserId == customerId && c.IsStatus == Status.Active , false).SingleOrDefaultAsync();
        public async Task<Address> GetAddressCustomer( int customerId)
        => await FindByCondition(c => c.UserId == customerId , false).SingleOrDefaultAsync();
        public void AddAddress(Address address) => Create(address);
        public void DeleteAddress(Address address) => Delete(address);
    }
}
