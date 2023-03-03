using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Enums
{
    public enum PaymentStatus
    {
        braintree = 1,
        stripe,
        QatarCharity,
        cash_on_delivery,
        instamojo,
        hyperpay,
        razor_pay,
        pay_tm,
        banktransfer,
        paystack,
        midtrans
    }
}
