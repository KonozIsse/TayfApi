using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Models.Enums;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CartRepository : RepositoryBase<Cart>, ICartRepository
    {
        public CartRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<Cart> GetCustomerProduct(int productId, int customerId , bool trackChanges)
        => await FindByCondition(x => x.CustomerId == customerId// && x.ProdId == productId
        && x.IsStatus == Status.Active ,trackChanges).FirstOrDefaultAsync();
        public async Task<List<Cart>> CartsNotActiveCustomer(int customerId)
         => await FindByCondition(x => x.CustomerId == customerId && x.IsStatus == Status.NotActive, false).ToListAsync();
         public async Task<List<Cart>> GetCartsToCustomerId(int customerId)
         => await FindByCondition(x => x.CustomerId == customerId && x.IsStatus == Status.Active, false).ToListAsync();
        public async Task<List<Cart>> GetCartsToStoreId(int storeId)
        => await FindByCondition(x => x.StoreId == storeId && x.IsStatus == Status.Active, false).ToListAsync();
        public async Task<Cart> GetCartId(int id , bool trackChanges)
        => await FindByCondition(x => x.Id == id  , trackChanges).FirstOrDefaultAsync();
        public async Task<List<Cart>> GetCarts()
       => await FindAll(false).ToListAsync();
        public void AddCart(Cart cart) => Create(cart);
        public void DeleteCart(Cart cart) => Delete(cart);
        public int GetCart(int custmer)
       => FindByCondition(c => c.CustomerId == custmer, false).GroupBy(x => x.ProdId).Select(x => x.First()).Count();
        public int CartCount()
        {
            return FindAll(false).ToList().GroupBy(x => x.ProdId).Select(x => x.First()).Count();
        }

    }
}
