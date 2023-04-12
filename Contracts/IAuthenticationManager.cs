using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IAuthenticationManager
    {
        Task<string> CreateToken();
        string GenerateRefreshToken();
        Task<bool> ValidateUser(UserForAuthenticationDto user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
