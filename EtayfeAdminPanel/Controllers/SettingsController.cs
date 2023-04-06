using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Entities.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : MyBaseController
    {
        public SettingsController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("getPages")]
        public async Task<IActionResult> GetAllPages(string search ,string filter, [FromQuery] PostsParameters postsParameters)
        {
            var result = await _homeBL.GetAllPages(search, filter,GetLanguage(), postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
       
        [HttpPut("updatePage")]
        public async Task<IActionResult> EditPage(EditPageDto update)
        {
            var result = await _homeBL.EditPage(update);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
       //---------------------------------------------------------------
        [HttpGet("getSettings")]
        public async Task<IActionResult> GetAllSettings()
        {
            var result = await _homeBL.GetAllSettings();
            return Ok(result);
        }

        [HttpPut("EditSettingStore")]
        public async Task<IActionResult> EditSettingStore(SettingStoreVM update)
        {
            var result = await _homeBL.EditSettingStore(update);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("EditSetting")]
        public async Task<IActionResult> EditSetting(SettingVM update)
        {
            var result = await _homeBL.EditSetting(update);
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
