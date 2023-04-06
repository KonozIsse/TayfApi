using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingMobController : MyBaseController
    {
        public SettingMobController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetSliderMobile( string search,[FromQuery] PostsParameters postsParameters)
        {
            var result = await _homeBL.GetSliderMobile(GetLanguage(), search, postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateSlider(CreateSliderDto create)
        {
           
            var result = await _homeBL.AddSliderMobile(GetCurrentUserId(), create);
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
        public async Task<IActionResult> UpdateSlider(UpdateSliderDto update)
        {
            var result = await _homeBL.UpdateSlider(GetCurrentUserId(), update);
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
        public async Task<IActionResult> DeleteSlide(int id)
        {
            var result = await _homeBL.DeleteSlide(id);
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
