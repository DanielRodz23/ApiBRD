using ApiBRD.Models.DTOs;
using ApiBRD.Models.Entities;
using ApiBRD.Repositories;
using AutoMapper;
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
        private readonly IMapper mapper;

        public MenuController(Repository<Menudeldia> repositoryMenu, IMapper mapper)
        {
            this.repositoryMenu = repositoryMenu;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var datos = repositoryMenu.Context.Menudeldia.Include(x => x.IdProductoNavigation).ToList().Select(x => mapper.Map<MenuDelDiaDTO>(x));
            //datos.ForEach(x => x.IdProductoNavigation.Menudeldia = null);
            //datos.ForEach(x => mapper.Map<MenuDelDiaDTO>(x));
            return Ok(datos);
        }
        [HttpPost]
        public async Task<IActionResult> Post(int[] ids)
        {
            if (ids.Length == 0)
            {
                return BadRequest();
            }

            ids = ids.Where(x => x != 0).Distinct().ToArray();


            var alldata = repositoryMenu.GetAll().ToList();

            foreach (var item in alldata)
            {
                repositoryMenu.Context.Remove(item);
                repositoryMenu.Context.SaveChanges();
            }


            for (int i = 0; i < ids.Length; i++)
            {
                if (!repositoryMenu.Context.Producto.Any(x => x.Id == ids[i]))
                {
                    continue;
                }

                var newmenu = new Menudeldia() { IdProducto = ids[i] };
                repositoryMenu.Context.Menudeldia.Add(newmenu);
            }
            repositoryMenu.Context.SaveChanges();

            return Ok();
        }
    }
}
