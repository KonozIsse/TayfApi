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
        public async Task<Cart> GetCartCustomerProduct(int productId, int customerId , bool trackChanges)
        => await FindByCondition(x => x.CustomerId == customerId && x.ProdId == productId,trackChanges).FirstOrDefaultAsync();
        public async Task<List<Cart>> GetCartsActiveCustomerId(int customerId)
        => await FindByCondition(x => x.CustomerId == customerId && x.IsStatus == Status.Active, false).ToListAsync();
        public async Task<List<Cart>> CartsNotActiveCustomer(int customerId)
         => await FindByCondition(x => x.CustomerId == customerId && x.IsStatus == Status.NotActive, false).ToListAsync();
         public async Task<List<Cart>> GetCartsToCustomerId(int customerId)
         => await FindByCondition(x => x.CustomerId == customerId , false).Include(c=>c.Store).Include(c=>c.Product).ThenInclude(c=>c.Images).ToListAsync();
        public async Task<List<Cart>> GetCartsToStoreId(int storeId)
        => await FindByCondition(x => x.StoreId == storeId && x.IsStatus == Status.Active, false).ToListAsync();
        public async Task<List<Cart>> GetCartsToStoreCustomer(int storeId , int customerId)
        => await FindByCondition(x => x.StoreId == storeId && x.CustomerId == customerId, false).ToListAsync();
        public async Task<Cart> GetCartId(int id , bool trackChanges)
        => await FindByCondition(x => x.Id == id  , trackChanges).FirstOrDefaultAsync();
        public void AddCart(Cart cart) => Create(cart);
        public void DeleteCart(Cart cart) => Delete(cart);

       
    }
}
