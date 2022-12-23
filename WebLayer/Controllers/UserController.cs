
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
        
        public UserController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            
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
