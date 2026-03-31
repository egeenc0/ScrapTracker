using Microsoft.AspNetCore.Mvc;

namespace HurdaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAdmin()
        {
            return Ok("Admin controller çalışıyor");
        }

        
    }
}