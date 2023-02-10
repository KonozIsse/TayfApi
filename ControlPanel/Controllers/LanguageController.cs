using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguageController : MyBaseController
    {
        public LanguageController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetAllLanguages(string search ,[FromQuery] PostsParameters postsParameters)
        {
            var result = await _homeBL.GetAllLanguages(search, GetLanguage(),postsParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateLanguage(UpdateLanguageDto update)
        {
            var result = await _homeBL.UpdateLanguage(update);
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
