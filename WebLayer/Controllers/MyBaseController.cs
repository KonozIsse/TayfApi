using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Entities.Models;
using Microsoft.AspNetCore.Hosting;
using BusinessLogic.ApiClasses;
using BusinessLogic;

namespace WebLayer.Controllers
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
        protected readonly LocService _locService;
        //protected readonly ImageBl _imageApi;
        //protected readonly NewsBL _newsBL;
        //protected readonly UserBL _userApi;
        //protected readonly HomeBL _homeBL;
        //protected readonly ProductBL _productBl;
        // protected readonly IWebHostEnvironment _webHostEnvironment;

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
            //_userApi = provider.GetService<UserBL>();
            //_homeBL = provider.GetService<HomeBL>();
            //_imageApi = provider.GetService<ImageBl>();
            //_newsBL = provider.GetService<NewsBL>();
            //_productBl = provider.GetService<ProductBL>();
            // _webHostEnvironment = provider.GetService<>;
        }
        [NonAction]
        public User GetCurrentUser()
        {
            var userName = User.Identity.Name;
            var user = _userManager.FindByNameAsync(userName).Result;
            return user;

        }
        [NonAction]
        public int GetCurrentUserId()
        {
            return GetCurrentUser().Id;
        }

    }
}
