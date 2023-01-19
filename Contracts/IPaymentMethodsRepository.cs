using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IPaymentMethodsRepository
    {
        Task<List<PaymentMethods>> GetPaymentMethods(string search);
        Task<List<PaymentMethods>> GetPaymentsByVendor(int vendorId);
    }
}
