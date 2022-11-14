using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CustomerStoreRepository : RepositoryBase<CustomerStore>, ICustomerStoresRepository
    {
        public CustomerStoreRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<CustomerStore> GetCustomerStoreId(int id, bool trackChanges)
        => await FindByCondition(r => r.Id == id, trackChanges).FirstOrDefaultAsync();
        public async Task<List<CustomerStore>> GetCustomersStoreId (int storeId)
        => await FindByCondition(c => c.StoreId == storeId, false).ToListAsync();
        public async Task<List<CustomerStore>> GetStoresCustomerId(int customerId)
        => await FindByCondition(r => r.CustomerId == customerId, false).ToListAsync();
        public void AddCustomerStore(CustomerStore customerStore) => Create(customerStore);
        public void DeleteCustomerStore(CustomerStore customerStore) => Delete(customerStore);
    }
}
