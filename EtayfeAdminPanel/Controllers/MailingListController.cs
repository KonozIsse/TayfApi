using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EtayfeAdminPanel.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MailingListController : MyBaseController
    {
        public MailingListController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("getEmails")]
        public async Task<IActionResult> GetMailLists()
        {
            var result = await _homeBL.GetMailListstest(GetCurrentUserId());
            //Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.MetaData));
            return Ok(result);
        }
        [HttpGet("getTemplate")]
        public async Task<IActionResult> GetAllMessageTemplates()
        {
            var result = await _homeBL.GetAllMessageTemplates();
            return Ok(result);
        }
       
        [HttpPut("updateTemplate")]
        public async Task<IActionResult> UpdateTemplate(UpdateTemplateDto update)
        {
            var result = await _homeBL.UpdateTemplate(update);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpDelete("deleteEmail")]
        public async Task<IActionResult> RemoveMailList(int id)
        {
            var result = await _homeBL.RemoveMailList(id);
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
