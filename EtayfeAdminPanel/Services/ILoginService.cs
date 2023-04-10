using Entities.DataTransferObjects;
using Entities.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtayfeAdminPanel.Services
{
    public interface ILoginService
    {
        Task Login(string token);
        Task Logout();
        //Task<string> RefreshToken1();

        //Task<string> TryRefreshToken();

        //Task<string> TryForceRefreshToken();

    }
}
