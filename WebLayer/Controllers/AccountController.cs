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
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : MyBaseController
    {
        public AccountController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
        {
            if (!await _authManager.ValidateUser(user))
            {
                _logger.LogWarn($"{nameof(Authenticate)}: Authentication failed. Wrong user name or password.");
                return Unauthorized();
            }
            return Ok(new { Token = await _authManager.CreateToken() });
        }

        [HttpPost("registe-customer")]
        public async Task<IActionResult> RegisterCustomer([FromForm] CreateCustomerDto userForRegistration)
        {
            var result = await _userBL.RegisterCustomer(userForRegistration);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            else
            {
                return Ok(result.Message);
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
             _emailSender.SendEmail(message);
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
