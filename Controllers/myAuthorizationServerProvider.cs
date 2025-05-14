using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calyx_Solutions.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class myAuthorizationServerProvider : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Success");
        }
    }
}
