using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICustomerStoresRepository
    {
        Task<CustomerStore> GetCustomerStoreId(int id, bool trackChanges);
        Task<List<CustomerStore>> GetCustomersStoreId(int storeId);
        Task<List<CustomerStore>> GetStoresCustomerId(int customerId);
        void AddCustomerStore(CustomerStore customerStore);
        void DeleteCustomerStore(CustomerStore customerStore);
    }
}
