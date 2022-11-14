using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICustomerProductRepository
    {
        Task<CustomerProduct> GetCustomerIdProduct(int productId, int customerId);
        Task<CustomerProduct> GetCustomerProductId(int customerProductId, bool trackChanges);
        Task<List<CustomerProduct>> GetCustomersProductId(int productId);
        Task<List<CustomerProduct>> GetProductsCustomerId(int customerId);
        Task<List<CustomerProduct>> GetStoreCustomer(int customerId, int storeId);
        void AddCustomerProduct(CustomerProduct customerProduct);
        void DeleteCustomerProduct(CustomerProduct customerProduct);
    }  
    public interface ICustomerAttributesProductRepository
    {
        Task<List<CustomerAttributesProduct>> GetAllAttributesCustomerProductId(int customerProductId, int productId);
        Task<List<CustomerAttributesProduct>> GetAllAttributesCustomerProduct(int customerProductId);
        Task DeleteAttributesCustomerProductList(List<int> Ids);
        void AddAttributeCustomerProduct(CustomerAttributesProduct customerAttributesProduct);
        void DeleteAttributeCustomerProduct(CustomerAttributesProduct customerAttributesProduct);
    }
}
