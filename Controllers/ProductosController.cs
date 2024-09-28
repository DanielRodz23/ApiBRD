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
    public class ProductosController : ControllerBase
    {
        public ProductosController(LabsystePwaBrdContext context, IHubContext<CategoriaHub> hubContext)
        {
            Context = context;
            HubContext = hubContext;
        }

        public LabsystePwaBrdContext Context { get; }
        public IHubContext<CategoriaHub> HubContext { get; }


        [HttpGet]
        public IActionResult GetAll(int idcategoria)
        {
            var productos = Context.Producto.Where(x => x.IdCategoria == idcategoria);

            return Ok(productos.Select(x => new
            {
                x.Id,
                x.IdCategoria,
                x.Disponible,
                x.Nombre,
                x.Precio
            }));
        }


        [HttpPost]
        public async Task<IActionResult> AgregarProducto(ProductoDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                return BadRequest("Ingrese un nombre para el producto.");
            }

            Producto p = new()
            {
                Id = 0,
                Nombre = dto.Nombre,
                Precio = dto.Precio,
                Disponible = dto.Disponible,
                IdCategoria = dto.IdCategoria
            };
            Context.Add(p);
            Context.SaveChanges();

            await HubContext.Clients.All.SendAsync("NuevoProducto",p);
            return Ok("El producto fue agregado con exito.");
        }

        [HttpPut]
        public async Task<IActionResult> EditarProducto(ProductoDTO dto)
        {
            var productoexistente = Context.Producto.Find(dto.Id);

            if(productoexistente == null)
            {
                return NotFound("El producto que intenta editar no fue encontrado.");
            }
            productoexistente.Nombre = dto.Nombre;
            productoexistente.Precio = dto.Precio;
            productoexistente.Disponible = dto.Disponible;
            Context.Update(productoexistente);

            int total = Context.SaveChanges();

            if(total > 0)
            {
                await HubContext.Clients.All.SendAsync("ProductoEditado", new
                {
                    productoexistente.Id,
                    productoexistente.Nombre,
                    productoexistente.Precio,
                    productoexistente.Disponible,
                    productoexistente.IdCategoria
                });
            }

            return Ok("El producto fue editado con exito.");
        }

        [HttpDelete]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var productoexistente = Context.Producto.Find(id);

            if(productoexistente == null)
            {
                return NotFound("No se pudo encontrar el producto que desea eliminar.");
            }

            Context.Remove(productoexistente);
            Context.SaveChanges();

            await HubContext.Clients.All.SendAsync("ProductoEliminado", productoexistente.Id);

            return Ok("El producto fue eliminado con exito.");
        }
    }
}
