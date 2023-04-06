using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICartRepository
    {
        Task<Cart> GetCartCustomerProduct(int productId, int customerId, bool trackChanges);
        Task<List<Cart>> GetCartsToCustomerId(int customerId);
        Task<List<Cart>> GetAllCarts(bool trackChanges);
        Task<List<Cart>> GetCartsToStoreId(int storeId);
        Task<List<Cart>> CartsNotActiveCustomer(int customerId);
        Task<Cart> GetCartId(int id, bool trackChanges);
        Task<List<Cart>> GetCartsToStoreCustomer(int storeId, int customerId);
        void AddCart(Cart cart);
        void DeleteCart(Cart cart);
    }
}
