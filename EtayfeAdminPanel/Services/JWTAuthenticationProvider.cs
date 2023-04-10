using Blazored.LocalStorage;
using Entities.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace EtayfeAdminPanel.Services
{

    public class JWTAuthenticationProvider : AuthenticationStateProvider, ILoginService
    {
        //private readonly HttpClient _httpClient;
        //private readonly ILocalStorageService _localStorage;

        //public JWTAuthenticationProvider(
        //    HttpClient httpClient,
        //    ILocalStorageService localStorage)
        //{
        //    _httpClient = httpClient;
        //    _localStorage = localStorage;
        //}

        //public void MarkUserAsAuthenticated(string userName)
        //{
        //    var authenticatedUser = new ClaimsPrincipal(
        //        new ClaimsIdentity(new[]
        //        {
        //            new Claim(ClaimTypes.Name, userName)
        //        }, "apiauth"));

        //    var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

        //    NotifyAuthenticationStateChanged(authState);
        //}

        //public void MarkUserAsLoggedOut()
        //{
        //    var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        //    var authState = Task.FromResult(new AuthenticationState(anonymousUser));

        //    NotifyAuthenticationStateChanged(authState);
        //}

        //public async Task<ClaimsPrincipal> GetAuthenticationStateProviderUserAsync()
        //{
        //    var state = await this.GetAuthenticationStateAsync();
        //    var authenticationStateProviderUser = state.User;
        //    return authenticationStateProviderUser;
        //}

        //public ClaimsPrincipal AuthenticationStateUser { get; set; }

        //public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        //{
        //    var savedToken = await _localStorage.GetItemAsync<string>("");
        //    if (string.IsNullOrWhiteSpace(savedToken))
        //    {
        //        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        //    }
        //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
        //    var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(GetClaimsFromJwt(savedToken), "jwt")));
        //    AuthenticationStateUser = state.User;
        //    return state;
        //}

        //private IEnumerable<Claim> GetClaimsFromJwt(string jwt)
        //{
        //    var claims = new List<Claim>();
        //    var payload = jwt.Split('.')[1];
        //    var jsonBytes = ParseBase64WithoutPadding(payload);
        //    var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        //    if (keyValuePairs != null)
        //    {
        //        keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

        //        if (roles != null)
        //        {
        //            if (roles.ToString().Trim().StartsWith("["))
        //            {
        //                var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

        //                claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
        //            }
        //            else
        //            {
        //                claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
        //            }

        //            keyValuePairs.Remove(ClaimTypes.Role);
        //        }


        //        claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
        //    }
        //    return claims;
        //}

        //private byte[] ParseBase64WithoutPadding(string base64)
        //{
        //    switch (base64.Length % 4)
        //    {
        //        case 2: base64 += "=="; break;
        //        case 3: base64 += "="; break;
        //    }

        //    return Convert.FromBase64String(base64);
        //}

        private readonly IJSRuntime js;
        private readonly HttpClient httpClient;
        private static readonly string TOKENKEY = "TOKENKEY";

        private AuthenticationState Anonymous => new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public JWTAuthenticationProvider(IJSRuntime js, HttpClient httpClient)
        {
            this.js = js;
            this.httpClient = httpClient;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = "";// await js.GetFromLocalStorage(TOKENKEY);

            if (string.IsNullOrEmpty(token))
            {
                return Anonymous;
            }

            return BuildAuthenticationState(token);
        }

        public async Task Login(string token)
        {
            await js.SetInLocalStorage(TOKENKEY, token);
            var authState = BuildAuthenticationState(token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }
        public async Task Language(string code)
        {
            await js.SetInLocalStorage("Lang", code);
        }

        public async Task Logout()
        {
            httpClient.DefaultRequestHeaders.Authorization = null;
            await js.RemoveItem(TOKENKEY);
            NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
        }

        private AuthenticationState BuildAuthenticationState(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}


