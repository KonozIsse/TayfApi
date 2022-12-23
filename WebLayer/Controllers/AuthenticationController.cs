using AutoMapper;
using BusinessLogic.ApiClasses;
using Contracts;
using Entities;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;

namespace WebLayer.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : MyBaseController
    {
        public AuthenticationController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromForm] CreateCustomerDto userForRegistration)
        {
            var user = _mapper.Map<User>(userForRegistration);
            var result = await _userManager.CreateAsync(user, userForRegistration.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return StatusCode(201);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
        {
            if (!await _authManager.ValidateUser(user))
            {
                _logger.LogWarn($"{nameof(Authenticate)}: Authentication failed. Wrong user name or password.");
                return Unauthorized();
            }
            //return Ok(new { usToken = await _authManager.CreateToken() });
            return Ok(new { Token = await _authManager.CreateToken() });

        }
        [HttpPost]
        [Route("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                if (model is null)
                    return BadRequest("No data found!");
               var user = GetCurrentUser();
                if (user == null)
                    return BadRequest("No user found!");


                var checkOldPassword =
                    await _signInManager.PasswordSignInAsync(user.UserName, model.OldPassword, false, false);


                if (!checkOldPassword.Succeeded)
                    return BadRequest(_locService.GetLocalizedStringValue("Old password does not matched."));

                if (model.ConfirmPassword != model.NewPassword)
                    return BadRequest(_locService.GetLocalizedStringValue("New password does not matched Confirm Password"));

                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (string.IsNullOrEmpty(resetToken))
                    return BadRequest(_locService.GetLocalizedStringValue("Error while generating reset token."));

                var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

                if (result.Succeeded)
                    return Ok();
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("TestSendEmail")]
        public IActionResult TestSendEmail()
        {
            var rng = new Random();
            var message = new Message(new string[] { "osama_rifag@hotmail.com", "ahmed.zaalan@gmail.com" }, "Test email", "This is the content from our email.");
            _emailSender.SendEmail(message);
            return Ok();
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
            var callback = Url.Action(nameof(ResetPassword), "AuthenticationController", new { token, email = user.Email }, Request.Scheme);
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

        [HttpPost]
        [AllowAnonymous]
        [Route("create-role")]
        public async Task<IActionResult> CreateRole(string name)
        {
          await  _roleManager.CreateAsync(new Role
            {
                Name = name,
                NormalizedName = name.ToUpper()
            }) ;
            return Ok();
        }
    }

}
