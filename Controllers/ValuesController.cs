using ApiBRD.Models.DTOs;
using ApiBRD.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiBRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly LabsystePwaBrdContext context;

        public ValuesController(LabsystePwaBrdContext context)
        {
            this.context = context;
        }

        [HttpGet("/")]
        public IActionResult GetAll()
        {
            return Ok("Working");
        }

       
    
    }
}
