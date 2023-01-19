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
    }
}
