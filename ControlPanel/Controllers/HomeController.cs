using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ControlPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : MyBaseController
    {
        public HomeController(IServiceProvider provider) : base(provider)
        {
        }
    }
}
