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
    public class CouponRepository : RepositoryBase<Coupon>, ICouponRepository
    {
        public CouponRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<IEnumerable<Coupon>> GetCoupons()
         => await FindAll(false).ToListAsync();
        public bool CheckExistCoupon(string code)
        => FindByCondition(x => x.CouponCode == code,false).Count() > 0;
        public async Task<Coupon> GetCouponId(int id , bool trackChanges)
        => await FindByCondition(x => x.Id == id, trackChanges).FirstOrDefaultAsync();
        public async Task<Coupon> GetCouponCodeNotFinished (string code)
        => await FindByCondition(x => x.CouponCode == code && x.ExpiryDate > EasternStandardTime(), false).FirstOrDefaultAsync();
        public async Task<Coupon> GetCouponIdNotFinished(int id)
       => await FindByCondition(x => x.Id == id && x.ExpiryDate > EasternStandardTime(), false).FirstOrDefaultAsync();
        public void AddCoupon(Coupon coupon) => Create(coupon);
        public void DeleteCoupon(Coupon coupon) => Delete(coupon);
    }
}
