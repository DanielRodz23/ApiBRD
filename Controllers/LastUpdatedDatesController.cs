using ApiBRD.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiBRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LastUpdatedDatesController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(LastUpdateManager.LoadData());
        }
    }
}
