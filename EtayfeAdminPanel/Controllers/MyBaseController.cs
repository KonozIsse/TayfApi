using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Entities.Models;
using BusinessLogic.ApiClasses;
using BusinessLogic;
namespace EtayfeAdminPanel.Controllers
{

    public class MyBaseController : ControllerBase
    {
        protected readonly ILoggerManager _logger;
        protected readonly IMapper _mapper;
        protected readonly UserManager<User> _userManager;
        protected readonly RoleManager<Role> _roleManager;
        protected readonly SignInManager<User> _signInManager;
        protected readonly IAuthenticationManager _authManager;
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IEmailSender _emailSender;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly LocService _locService;
        protected readonly NewsBL _newsBL;
        protected readonly HomeBL _homeBL;
        protected readonly ProductBL _productBL;
        protected readonly OrderBL _orderBL;
        protected readonly CartBL _cartBL;
        protected readonly UserBL _userBL;
        protected readonly ImageBL _imageBL;
        protected readonly LocationTaxBL _locationTaxBL;

        public MyBaseController(IServiceProvider provider)
        {
            _logger = provider.GetService<ILoggerManager>();

            _mapper = provider.GetService<IMapper>();

            _userManager = provider.GetService<UserManager<User>>();

            _roleManager = provider.GetService<RoleManager<Role>>();

            _authManager = provider.GetService<IAuthenticationManager>();

            _repositoryManager = provider.GetService<IRepositoryManager>();

            _signInManager = provider.GetService<SignInManager<User>>();

            _emailSender = provider.GetService<IEmailSender>();
            _locService = provider.GetService<LocService>();
            _homeBL = provider.GetService<HomeBL>();
            _newsBL = provider.GetService<NewsBL>();
            _productBL = provider.GetService<ProductBL>();
            _orderBL = provider.GetService<OrderBL>();
            _cartBL = provider.GetService<CartBL>();
            _userBL = provider.GetService<UserBL>();
            _imageBL = provider.GetService<ImageBL>();
            _locationTaxBL = provider.GetService<LocationTaxBL>();
        }
        [NonAction]
        public User GetCurrentUser()
        {
            var userName = User.Identity.Name;
            var user = _userManager.FindByNameAsync(userName).Result;
            return user;
        }
        [NonAction]
        public async Task Logout()
        {
            if (_signInManager.IsSignedIn(User))
            {
               await  _signInManager.SignOutAsync();
            }
        }
        [NonAction]
        public int GetCurrentUserId()
        {
            var user = GetCurrentUser();
            return user.Id;
        }
     
        [NonAction]
        public int GetCurrentCurrencyId()
        {
            var currency = _repositoryManager.Currency.GetDefaultCurrency(false).Result;
            return currency.Id;
        }
        [NonAction]
        public string GetLanguage()
        {
            var langusge = _repositoryManager.Language.GetDefaultLanguage(false).Result;
            return langusge.Code;
        }
    }
}
