using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Enums
{
   // [Flags]
    public enum UserType
    {
        Admin = 1,
        Customer,
        Store,
        //AdminAndStore = Admin & Store
    }
}
