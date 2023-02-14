using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Enums
{
    public enum NameTemplate
    {
        NewsLetterEmail=1,
        VerificationEmail,
        OrderShipped,
        OrderCompleted,
        OrderRejected,
        OrderRecieved,
        DeactiveAccount,
        ActiveAccount
    }
}
