using AutoMapper;
using Entities;
using Entities.DataTransferObjects;
using EtayfeAdminPanel.Model;
using EtayfeAdminPanel.Services;
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
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForAuthenticationDto userForAuthentication)
        {
            var validateUser = await _authManager.ValidateUser(userForAuthentication);
            if (!validateUser)
            {
                return Unauthorized(new TokenResponse { ErrorMessage = "Invalid Authentication" });
            }
            var token = await _authManager.CreateToken();
            var user = await _userManager.FindByNameAsync(userForAuthentication.UserName);
            user.RefreshToken = _authManager.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);
            return Ok(new TokenResponse { IsAuthSuccessful = true, Token = token, RefreshToken = user.RefreshToken });
        }

        [HttpGet("currentUser")]
        public async Task<IActionResult> GetCurrentUserHome()
        {
            var result = await _homeBL.GetCurrentUser(3);
             return Ok(result);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest tokenDto)
        {
            if (tokenDto is null)
            {
                return BadRequest(new TokenResponse { IsAuthSuccessful = false, ErrorMessage = "Invalid client request" });
            }
            var principal = _authManager.GetPrincipalFromExpiredToken(tokenDto.Token);
            var username = principal.Identity.Name;
            var user = await _userManager.FindByEmailAsync(username);
            if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest(new TokenResponse { IsAuthSuccessful = false, ErrorMessage = "Invalid client request" });
            }
            var token = await _authManager.CreateToken();
            user.RefreshToken = _authManager.GenerateRefreshToken();
            await _userManager.UpdateAsync(user);
            return Ok(new TokenResponse { Token = token, RefreshToken = user.RefreshToken, IsAuthSuccessful = true });
        }
        //@attribute [Authorize(Roles = "Administrator")]
       
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
