
using BusinessLogic;
using BusinessLogic.ApiClasses;
using Entities.DataTransferObjects;
using Entities.Exception;
using Entities.Models;
using EtayfeAdminPanel.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EtayfeAdminPanel.Services
{
    public class IdentityService: ITokenService
    {
        private readonly UserManager<User> _userManager;
        private readonly AuthenticationManager _authManager;

        public IdentityService(
            UserManager<User> userManager, AuthenticationManager authManager)
        {
            _userManager = userManager;
            _authManager = authManager;
        }
        public async Task<ExceptionModel<TokenResponse>> LoginAsync(UserForAuthenticationDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.UserName);
            if (user == null)
            {
                return new ExceptionModel<TokenResponse>(null, "Wrong user name", false);
            }
            if (!await _authManager.ValidateUser(model))
            {
                return new ExceptionModel<TokenResponse>(null, "Wrong user name or password.", false);
            }
            var token = await _authManager.CreateToken();

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);
            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, UserImageURL = user.Avater };
            return new ExceptionModel<TokenResponse>(response);
        }
        public async Task<ExceptionModel<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model)
        {
            if (model is null)
            {
                
            }
            var userPrincipal = GetPrincipalFromExpiredToken(model.Token);
            var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
          
            if (user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            return new ExceptionModel<TokenResponse>(null, "Invalid Client Token.", false);
            var token = await _authManager.CreateToken();
            user.RefreshToken = GenerateRefreshToken();
            await _userManager.UpdateAsync(user);

            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
            return new ExceptionModel<TokenResponse>(response);
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Secret")),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
