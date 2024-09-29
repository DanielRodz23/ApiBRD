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
            var datos = repositoryMenu.Context.Menudeldia.Include(x => x.IdProductoNavigation).ToList().Select(x=>mapper.Map<MenuDelDiaDTO>(x));
            //datos.ForEach(x => x.IdProductoNavigation.Menudeldia = null);
            //datos.ForEach(x => mapper.Map<MenuDelDiaDTO>(x));
            return Ok(datos);
        }
        [HttpPost]
        public async Task<IActionResult> Post(int[] ids)
        {
            int cant = 5;

            if (ids.GroupBy(x => x).Count() < cant)
            {
                return BadRequest($"Deben ser {cant} productos");
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
