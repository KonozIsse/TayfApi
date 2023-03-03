using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using Entities.Models.Enums;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Repository
{
    public class PaymentMethodsRepository : RepositoryBase<PaymentMethods>, IPaymentMethodsRepository
    {
        public PaymentMethodsRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<List<PaymentMethods>> GetPaymentMethods(string search)
        {
            var payments = FindByCondition(r => r.IsStatus == Status.Active, false);
            if (!string.IsNullOrEmpty(search))
            {
                payments.Where(c => c.PaymentMethod.Contains(search));
            }
            return await payments.ToListAsync();
        }
        public async Task<PaymentMethods> GetPaymentsStatus(PaymentStatus key)
         => await FindByCondition(r => r.IsStatus == Status.Active && r.PaymentStatus == key, false).FirstOrDefaultAsync();
    }
}
