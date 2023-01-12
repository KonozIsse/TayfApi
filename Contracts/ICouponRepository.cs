using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICouponRepository
    {
        Task<IEnumerable<Coupon>> GetCoupons(string search);
        bool CheckExistCoupon(string code);
        Task<Coupon> GetCouponId(int id, bool trackChanges);
        Task<Coupon> GetCouponCodeNotFinished(string code);
        Task<Coupon> GetCouponIdNotFinished(int id);
        Task<Coupon> GetCouponCode(string code);
        void AddCoupon(Coupon coupon);
        void DeleteCoupon(Coupon coupon);
    }
}
