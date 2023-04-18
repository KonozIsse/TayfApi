using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageVendorsController : MyBaseController
    {
        public ManageVendorsController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetAllStores(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _userBL.GetAllStores(search, postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpGet("GetStores")]
        public async Task<IActionResult> GetStores()
        {
            var result = await _userBL.GetStores();
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateStore(CreateStoreDto create)
        {
            var result = await _userBL.AddStore(create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateStore(UpdateStoreDto update)
        {
            var result = await _userBL.UpdateStore(update);
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
        public async Task<IActionResult> DeleteStore(int id)
        {
            var result = await _userBL.DeleteStore(id);
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
