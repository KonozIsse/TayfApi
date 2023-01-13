using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ControlPanel.Controllers
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
            var result = await _productBL.AddInventory(GetAdminId(),GetStoreId(),create);
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
