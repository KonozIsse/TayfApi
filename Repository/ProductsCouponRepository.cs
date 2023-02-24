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
    public class ProductsCouponRepository : RepositoryBase<ProductsCoupon>, IProductsCouponRepository
    {
        public ProductsCouponRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }

        public void CreatProductsCouponRange(List<ProductsCoupon> Products) => CreateRange(Products);

        public void DeleteProductsCoupon(ProductsCoupon productsCoupon)=> Delete(productsCoupon);

        public async Task DeleteRowRange(List<int> Ids)
        {
            var result = await FindByCondition(c => Ids.Contains(c.Id), true).ToListAsync();
            DeleteRange(result);
        }

        public async Task<List<ProductsCoupon>> GetAllProductsCouponId(int couponId, bool trackChanges)
        => await FindByCondition(c => c.CouponId == couponId, trackChanges).ToListAsync();
        public async Task<ProductsCoupon> GetItemId(int id, bool trackChanges)
      => await FindByCondition(c => c.Id == id, trackChanges).FirstOrDefaultAsync();
    }
}
