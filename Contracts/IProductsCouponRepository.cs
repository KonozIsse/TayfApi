using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IProductsCouponRepository
    {
        Task<List<ProductsCoupon>> GetAllProductsCouponId(int couponId, bool trackChanges);
        void DeleteProductsCoupon(ProductsCoupon productsCoupon);
    }
}
