using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CustomerProductRepository : RepositoryBase<CustomerProduct>, ICustomerProductRepository
    {
        public CustomerProductRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<CustomerProduct> GetCustomerIdProduct(int productId, int customerId)
        => await FindByCondition(t => t.ProductId == productId && t.CustomerId == customerId, false).FirstOrDefaultAsync();
        public async Task<CustomerProduct> GetCustomerProductId(int customerProductId , bool trackChanges)
        => await FindByCondition(t => t.Id == customerProductId, trackChanges).FirstOrDefaultAsync();
        public async Task<List<CustomerProduct>> GetCustomersProductId(int productId)
        => await FindByCondition(t => t.ProductId == productId , false).ToListAsync();
        public async Task<List<CustomerProduct>> GetProductsCustomerId(int customerId)
        => await FindByCondition(t => t.CustomerId == customerId, false).ToListAsync();
        public async Task<List<CustomerProduct>> GetStoreCustomer (int customerId , int storeId)
       => await FindByCondition(t => t.CustomerId == customerId && t.StoreId == storeId, false).ToListAsync();
        public void AddCustomerProduct(CustomerProduct customerProduct) => Create(customerProduct);
        public void DeleteCustomerProduct(CustomerProduct customerProduct) => Delete(customerProduct);
    }
    public class CustomerAttributesProductRepository : RepositoryBase<CustomerAttributesProduct>, ICustomerAttributesProductRepository
    {
        public CustomerAttributesProductRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<List<CustomerAttributesProduct>> GetAllAttributesCustomerProductId(int customerProductId, int productId)
        => await FindByCondition(c => c.CustomerProductId == customerProductId && c.CustomerProduct.ProductId == productId, false).ToListAsync();

        public async Task<List<CustomerAttributesProduct>> GetAllAttributesCustomerProduct(int customerProductId)
        => await FindByCondition(c => c.CustomerProductId == customerProductId , false).ToListAsync();

        public async Task DeleteAttributesCustomerProductList(List<int> Ids)
        {
            var cartProducts = await FindByCondition(c => Ids.Contains(c.Id), true).ToListAsync();
            DeleteRange(cartProducts);
        }
        public void AddAttributeCustomerProduct(CustomerAttributesProduct customerAttributesProduct) => Create(customerAttributesProduct);
    
        public void DeleteAttributeCustomerProduct(CustomerAttributesProduct customerAttributesProduct) => Delete(customerAttributesProduct);
    }
}
