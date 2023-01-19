using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : MyBaseController
    {
        public ReportsController(IServiceProvider provider) : base(provider)
        {
        }
    }
}
