using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : MyBaseController
    {
        public AdminController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetAdminsStors()
        {
            var result = await _userBL.GetAdminsStores();
            return Ok(result);
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAdmin(CreateAdminDto item, int state, string street , string zip)
        {
            var result = await _userBL.RegisterUser(item, state, street, zip,GetLanguage());
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("editAdmin")]
        public async Task<IActionResult> EditAdmin(UpdateAdminDto update)
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

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveAdmin(int id)
        {
            var result = await _userBL.RemoveUserData(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            else
            {
                return Ok(result.Message);
            }
        }
        
        [HttpPost("createRole")]
        public async Task<IActionResult> AddRole(CreateRoleDto create)
        {
            var result = await _userBL.AddRole(create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpGet("getRoles")]
        public async Task<IActionResult> GetTypesStoreAdmin()
        {
            var result = await _userBL.GetTypesStoreAdmin();
            return Ok(result);
        }
        [HttpPut("editRole")]
        public async Task<IActionResult> EditRole(UpdateRoleDto update)
        {
            var result = await _userBL.EditRole(update);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpDelete("removeRole")]
        public async Task<IActionResult> RemoveRole(int id)
        {
            var result = await _userBL.DeleteRole(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            else
            {
                return Ok(result.Message);
            }
        }
        [HttpPost("AddPerssmionRole")]
        public async Task<IActionResult> AddPerssmionRole(int roleId, List<RoleLinksDto> RoleLinksDto)
        {
            var result = await _userBL.SaveRole(roleId, RoleLinksDto);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("EditPerssmionRole")]
        public async Task<IActionResult> EditPerssmionRole(int roleId, List<RoleLinksDto> RoleLinksDto)
        {
            var result = await _userBL.EditPermissionRoleId(roleId, RoleLinksDto);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
    }
}
