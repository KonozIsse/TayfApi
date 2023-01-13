using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAdminsController : MyBaseController
    {
        public UserAdminsController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpPut("update")]
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
       

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAdmin(int id)
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
    }
}
