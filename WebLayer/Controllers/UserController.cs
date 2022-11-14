using BusinessLogic;
using BusinessLogic.ApiClasses;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebLayer.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : MyBaseController
    {
        protected readonly NewsBL _newsBL;
        protected readonly HomeBL _homeBL; 
        protected readonly ProductBL _productBL;
        protected readonly UserBL _userBL;
        public UserController(IServiceProvider serviceProvider , NewsBL newsBL, HomeBL homeBL , UserBL userBL , ProductBL productBL) : base(serviceProvider)
        {
            _newsBL = newsBL;
            _homeBL = homeBL;
            _productBL = productBL;
            _userBL = userBL;
        }
        [HttpGet]
        [Route("user-details")]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            var userDetails = await _repositoryManager.User.GetActiveUserId(userId, false);
            var UserProfile = _mapper.Map<UserDto>(userDetails);
            return Ok(UserProfile);
        }
      

    }
}
