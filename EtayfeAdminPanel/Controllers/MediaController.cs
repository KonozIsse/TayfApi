using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using System;
using System.Threading.Tasks;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : MyBaseController
    {
        public MediaController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("get-images")]
        public async Task<IActionResult> GetImages(ImageCategory? category, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _imageBL.GetImages(category , GetCurrentUserId(), postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        } 
        [HttpGet("get-settings-image/{imgId}")]
        public async Task<IActionResult> GetSettingsByImg(int imgId)
        {
            var result = await _imageBL.GetImageSettingImg(imgId);
            return Ok(result);
        }
        [HttpPost("addImages")]
        public async Task<IActionResult> CreateImages([FromForm] CreateImageDto create)
        {
            var result = await _imageBL.CreateImages(GetCurrentUserId(), create);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("editSettingImage")]
        public async Task<IActionResult> EditImageSetting(UpdateImageSettingDto update)
        {
            var result = await _imageBL.EditImageSetting(update);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpDelete("deleteListImages")]
        public async Task<IActionResult> DeleteImages(List<int> Ids)
        {
            var result = await _imageBL.DeleteImageIds(Ids);
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
