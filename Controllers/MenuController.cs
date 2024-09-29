using ApiBRD.Models.Entities;
using ApiBRD.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var datos = repositoryMenu.Context.Menudeldia.Include(x => x.IdProductoNavigation);
            return Ok(datos);
        }
        [HttpPost]
        public async Task<IActionResult> Post(int[] ids)
        {
            
            if (ids.GroupBy(x => x).Count()<5)
            {
                return BadRequest("Deben ser 5 productos");
            }


            var alldata = repositoryMenu.GetAll().ToList();
            
            
            repositoryMenu.Context.RemoveRange(alldata);
            repositoryMenu.Context.SaveChanges();
            

            for (int i = 0; i < ids.Length; i++)
            {
                var newmenu = new Menudeldia { IdProducto = ids[i] };
                repositoryMenu.Insert(newmenu);
            }
            return Ok();
        }
    }
}
