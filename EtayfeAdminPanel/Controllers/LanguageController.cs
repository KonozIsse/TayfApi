using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguageController : MyBaseController
    {
        public LanguageController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("GetLanguages")]
        public async Task<IActionResult> GetLanguages()
        {
            var result = await _homeBL.GetLanguages(GetLanguage());
            return Ok(result);
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetAllLanguages(string search ,string filter,[FromQuery] PostsParameters postsParameters)
        {
            var result = await _homeBL.GetAllLanguages(search, filter, GetLanguage(),postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpGet("GetDefaultLanugage")]
        public async Task<IActionResult> GetDefaultLanugage()
        {
            var result = GetLanguage();
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateLanguage(int id ,UpdateLanguageDto update)
        {
            var result = await _homeBL.UpdateLanguage(id,update);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        } 
        [HttpPut("change-defaultLanguage")]
        public async Task<IActionResult> ChangeDefaultLanugage(int id)
        {
             await _homeBL.ChangeDefaultLanugage(id);
             return Ok();
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteLanguage(int id)
        {
            var result = await _homeBL.DeleteLanguage(id);
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
