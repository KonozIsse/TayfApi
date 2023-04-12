using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

namespace EtayfeAdminPanel.Services
{
    public class RefreshTokenService
    {
        private readonly JWTAuthenticationProvider _authProvider;
        private readonly ILoginService _authService;
        public RefreshTokenService(JWTAuthenticationProvider authProvider, ILoginService authService)
        {
            _authProvider = authProvider;
            _authService = authService;
        }
        public async Task<string> TryRefreshToken()
        {
            var authState = await _authProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var exp = user.FindFirst(c => c.Type.Equals("exp")).Value;
            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
            var timeUTC = DateTime.UtcNow;
            var diff = expTime - timeUTC;
            if (diff.TotalMinutes <= 2)
                return await _authService.RefreshToken();
            return string.Empty;
        }
    }
}
