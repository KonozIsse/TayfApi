using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EtayfeAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : MyBaseController
    {
        public InventoryController(IServiceProvider provider) : base(provider)
        {
        }
       
        [HttpPost("create")]
        public async Task<IActionResult> CreateInventory(CreateInventoryDto create)
        {
            var result = await _productBL.AddInventory(GetCurrentUserId(),create);
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
