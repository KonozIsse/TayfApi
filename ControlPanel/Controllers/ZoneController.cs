using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZoneController : MyBaseController
    {
        public ZoneController(IServiceProvider provider) : base(provider)
        {
        }
    }
}
