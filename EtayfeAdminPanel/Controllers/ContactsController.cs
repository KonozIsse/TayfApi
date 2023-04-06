using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : MyBaseController
    {
        public ContactsController(IServiceProvider provider) : base(provider)
        {
        }
        [HttpGet("get-contacts")]
        public async Task<IActionResult> GetAllContacts(string search,string filter,PostsParameters postsParameters)
        {
            var result = await _homeBL.GetAllContacts(search, filter,postsParameters);
            return Ok(result);
        }
        [HttpDelete("delete-contect")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var result = await _homeBL.DeleteContact(id);
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
