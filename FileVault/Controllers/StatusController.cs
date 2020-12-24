using Microsoft.AspNetCore.Mvc;

namespace FileVault.Controllers
{
    [ApiController]
    [Route("")]
    public class StatusController : Controller
    {
        [HttpGet("/server-status")]
        public IActionResult Status()
        {
            return Ok();
        }
    }
}