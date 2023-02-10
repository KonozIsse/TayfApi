using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingWebController : MyBaseController
    {
        public SettingWebController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("getSliderWeb")]
        public async Task<IActionResult> GetSliderWeb(string search ,[FromQuery] PostsParameters postsParameters)
        {
            var result = await _homeBL.GetSliderWeb(search,GetLanguage(), postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpPost("create-slider")]
        public async Task<IActionResult> CreateSlider(CreateSliderDto create)
        {

            var result = await _homeBL.AddSliderWeb(GetCurrentUserId(), create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("update-Slider")]
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
        [HttpDelete("delete-slider")]
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

        //----------------------------------------------------------
        [HttpGet("get-banners")]
        public async Task<IActionResult> GetBanners(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _homeBL.GetBanners(search, GetLanguage(), postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpPut("update-banner")]
        public async Task<IActionResult> UpdateBanner(UpdateBannerDto update)
        {
            var result = await _homeBL.UpdateBanner(update);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        //----------------------------------------------------------
        [HttpGet("get-services")]
        public async Task<IActionResult> GetServices(string search, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _homeBL.GetServices(search, GetLanguage(), postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpPut("update-service")]
        public async Task<IActionResult> UpdateService(UpdateServiceDto update)
        {
            var result = await _homeBL.UpdateService(update);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpDelete("delete_service")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var result = await _homeBL.DeleteService(id);
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
