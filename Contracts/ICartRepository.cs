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
        Task<Cart> GetCustomerProduct(int productId, int customerId, bool trackChanges);
        Task<List<Cart>> GetCartsToCustomerId(int customerId);
        Task<List<Cart>> GetCartsToStoreId(int storeId);
        Task<List<Cart>> CartsNotActiveCustomer(int customerId);
        Task<Cart> GetCartId(int id, bool trackChanges);
        Task<List<Cart>> GetCarts();
        void AddCart(Cart cart);
        void DeleteCart(Cart cart);
        int GetCart(int custmer);
        int CartCount();
    }
}
