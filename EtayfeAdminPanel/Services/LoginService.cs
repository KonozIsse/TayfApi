using Blazored.LocalStorage;
using BusinessLogic.ApiClasses;
using Entities.DataTransferObjects;
using Entities.Exception;
using EtayfeAdminPanel.Model;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using System.Net.Http.Headers;

namespace EtayfeAdminPanel.Services
{
    //public class LoginService : ILoginService
    //{
    //    private readonly ITokenService _identityService;
    //    private readonly HttpClient _httpClient;
    //    private readonly ILocalStorageService _localStorage;
    //    private readonly AuthenticationStateProvider _authenticationStateProvider;
    //    public static string AuthToken = "authToken";
    //    public static string RefreshToken = "refreshToken";
    //    public LoginService(ITokenService identityService, 
    //        ILocalStorageService localStorage,
    //        AuthenticationStateProvider authenticationStateProvider,
    //        HttpClient httpClient)
    //    {
    //        _identityService = identityService;
    //        _localStorage = localStorage;
    //        _authenticationStateProvider = authenticationStateProvider;
    //        _httpClient = httpClient;
    //    }
    //    public async Task<BussnessResultModel> Login(UserForAuthenticationDto model)
    //    {
    //        var result = await _identityService.LoginAsync(model);
    //        if (result.Success)
    //        {
    //            var token = result.Data.Token;
    //            var refreshToken = result.Data.RefreshToken;
    //            await _localStorage.SetItemAsync(AuthToken, token);
    //            await _localStorage.SetItemAsync(RefreshToken, refreshToken);
    //            ((JWTAuthenticationProvider)this._authenticationStateProvider).MarkUserAsAuthenticated(model.UserName);
    //            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    //            return new BussnessResultModel(result);
    //        }
    //        else
    //        {
    //            return new BussnessResultModel(null,"",false);
    //        }
    //    }
    //    public async Task Logout()
    //    {
    //        await _localStorage.RemoveItemAsync(AuthToken);
    //        await _localStorage.RemoveItemAsync(RefreshToken);
    //        ((JWTAuthenticationProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
    //        _httpClient.DefaultRequestHeaders.Authorization = null;
    //    }
    //    public async Task<string> RefreshToken1()
    //    {
    //        var token = await _localStorage.GetItemAsync<string>(AuthToken);
    //        var refreshToken = await _localStorage.GetItemAsync<string>(RefreshToken);

    //        var response = await _identityService.GetRefreshTokenAsync(new RefreshTokenRequest { Token = token, RefreshToken = refreshToken });

            
    //        if (!response.Success)
    //        {
    //            throw new ApplicationException("Something went wrong during the refresh token action");
    //        }

    //        token = response.Data.Token;
    //        refreshToken = response.Data.RefreshToken;
    //        await _localStorage.SetItemAsync(AuthToken, token);
    //        await _localStorage.SetItemAsync(RefreshToken, refreshToken);
    //        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    //        return token;
    //    }

    //    public async Task<string> TryRefreshToken()
    //    {
    //        //check if token exists
    //        var availableToken = await _localStorage.GetItemAsync<string>(RefreshToken);
    //        if (string.IsNullOrEmpty(availableToken)) return string.Empty;
    //        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
    //        var user = authState.User;
    //        var exp = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
    //        var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
    //        var timeUTC = DateTime.UtcNow;
    //        var diff = expTime - timeUTC;
    //        if (diff.TotalMinutes <= 1)
    //            return await RefreshToken1();
    //        return string.Empty;
    //    }

    //    public async Task<string> TryForceRefreshToken()
    //    {
    //        return await RefreshToken1();
    //    }

    //}
}
