using ApiBRD.Models.Entities;
using ApiBRD.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiBRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly Repository<Menudeldia> repositoryMenu;

        public MenuController(Repository<Menudeldia> repositoryMenu)
        {
            this.repositoryMenu = repositoryMenu;
        }
        [HttpGet]
        public async Task<IActionResult> Get() 
        {
            return Ok();
        }
    }
}
