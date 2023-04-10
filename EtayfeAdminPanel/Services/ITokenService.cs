using Entities.DataTransferObjects;
using EtayfeAdminPanel.Model;
using Entities.Exception;
namespace EtayfeAdminPanel.Services
{
    public interface ITokenService
    {
        Task<ExceptionModel<TokenResponse>> LoginAsync(UserForAuthenticationDto model);

        Task<ExceptionModel<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model);
    }
}
