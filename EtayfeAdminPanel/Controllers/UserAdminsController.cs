using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAdminsController : MyBaseController
    {
        public UserAdminsController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpPut("update-admin")]
        public async Task<IActionResult> UpdateAdmin(UpdateAdminDto update)
        {
            var result = await _userBL.EditAdmin(update);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
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

        [HttpDelete("delete-admin")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var result = await _userBL.RemoveUser(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            else
            {
                return Ok(result.Message);
            }
        }
    }
}
