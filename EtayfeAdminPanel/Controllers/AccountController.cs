using AutoMapper;
using Entities;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : MyBaseController
    {
        public AccountController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        [HttpGet("currentUser")]
        public async Task<IActionResult> GetCurrentUserHome()
        {
            var result = await _homeBL.GetCurrentUser(GetCurrentUserId());
             return Ok(result);
        }

       
        [HttpGet("GetLanguages")]
        public async Task<IActionResult> GetLanguages()
        {
            var result = await _homeBL.GetLanguages("en");
            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] UserForAuthenticationDto user)
        {
            var result = await _userBL.ValidateUser(user);
            if (result.Success)
            { 
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordDto forgotPasswordModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
            if (user == null)
                return Ok(_locService.GetLocalizedStringValue("user is not found !"));
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callback = Url.Action(nameof(ResetPassword), "AccountController", new { token, email = user.Email }, Request.Scheme);
            var message = new Message(new string[] { user.Email }, "Reset password token", "<a href=\"" + callback + "\">click here</a>");
            await _emailSender.SendEmailAsync(message);
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
                return Ok(_locService.GetLocalizedStringValue("user is not found !"));
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Code, resetPasswordModel.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return Ok();
        }

    }

}
