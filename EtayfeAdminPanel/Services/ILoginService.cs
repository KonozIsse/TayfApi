using Entities.DataTransferObjects;
using Entities.Exception;
using EtayfeAdminPanel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtayfeAdminPanel.Services
{
    public interface ILoginService
    {
        Task<TokenResponse> Login(UserForAuthenticationDto token);
        Task Logout();
        Task<string> RefreshToken();
    }
}
