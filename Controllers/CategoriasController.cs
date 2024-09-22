using ApiBRD.Models.DTOs;
using ApiBRD.Models.Entities;
using ApiBRD.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ApiBRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {

        public LabsystePwaBrdContext context { get; }
        public IHubContext<CategoriaHub> HubContext { get; }


        public CategoriasController(LabsystePwaBrdContext context, IHubContext<CategoriaHub> hubContext)
        {
            this.context = context;
            HubContext = hubContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var categorias = context.Categoria;

            return Ok(categorias.Select(x => new
            {
                x.Id,
                x.Nombre,
                x.Producto
            }));
        }


        [HttpPost]
        public async Task<IActionResult> Post(CategoriaDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                return BadRequest("Ingrese el nombre de la categoria.");
            }

            Categoria d = new()
            {
                Id = 0,
                Nombre = dto.Nombre
            };
            context.Add(d);
            context.SaveChanges();

            await HubContext.Clients.All.SendAsync("NuevaCategoria", d);
            return Ok("La categoria fue agregada con exito.");
        }

        [HttpPut]
        public async Task<IActionResult> EditarCategoria(CategoriaDTO dto)
        {
            var categoria = context.Categoria.Find(dto.Id);

            if(categoria == null)
            {
                return NotFound("La categoria que intenta editar no fue encontrada.");
            }
            categoria.Nombre = dto.Nombre;
            context.Update(categoria);
            int total = context.SaveChanges();

            if(total > 0)
            {
                await HubContext.Clients.All.SendAsync("Categoria Editada", new
                {
                    categoria.Id,
                    categoria.Nombre,
                });
            }

            return Ok("La categoria fue editada con exito");
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var categoriaExistente = context.Categoria.Find(id);
            if(categoriaExistente == null)
            {
                return NotFound("No se pudo encontrar la categoria que desea eliminar.");
            }

            context.Remove(categoriaExistente);
            context.SaveChanges();

            await HubContext.Clients.All.SendAsync("CategoriaEliminada", categoriaExistente.Id);

            return Ok("La categoria fue eliminada con exito.");
        }
    }
}
