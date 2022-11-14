using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using Entities.Models.Enums;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class PaymentMethodsRepository : RepositoryBase<PaymentMethods>, IPaymentMethodsRepository
    {
        public PaymentMethodsRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<List<PaymentMethods>> GetPaymentMethods()
         => await FindByCondition(r => r.IsStatus == Status.Active, false).ToListAsync();
        public async Task<List<PaymentMethods>> GetPaymentsByVendor(int vendorId)
         => await FindByCondition(r => r.IsStatus == Status.Active && r.StoreId == vendorId, false).ToListAsync();
    }
}
