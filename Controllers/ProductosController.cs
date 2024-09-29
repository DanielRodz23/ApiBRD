using ApiBRD.Models.DTOs;
using ApiBRD.Models.Entities;
using ApiBRD.Repositories;
using ApiBRD.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ApiBRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly Repository<Producto> repository;
        private readonly IHubContext<CategoriaHub> hubContext;
        private readonly IMapper mapper;

        public ProductosController(Repository<Producto> repository, IHubContext<CategoriaHub> hubContext, IMapper mapper)
        {
            this.repository = repository;
            this.hubContext = hubContext;
            this.mapper = mapper;
        }



        [HttpGet]
        public IActionResult GetAll()
        {
            var data = repository.GetAll().Select(x => mapper.Map<ProductoDTO>(x));
            return Ok(data);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var dato = repository.GetById(id);
            return Ok(mapper.Map<ProductoDTO>(dato));
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
                Nombre = dto.Nombre,
                Precio = dto.Precio,
                Disponible = dto.Disponible,
                IdCategoria = dto.IdCategoria
            };
            repository.Insert(p);

            await hubContext.Clients.All.SendAsync("NuevoProducto", p);

            return Ok("El producto fue agregado con exito.");
        }

        [HttpPut]
        public async Task<IActionResult> EditarProducto(ProductoDTO dto)
        {
            var productoexistente = repository.GetById(dto.Id);

            if (productoexistente == null)
            {
                return NotFound("El producto que intenta editar no fue encontrado.");
            }
            productoexistente.Nombre = dto.Nombre;
            productoexistente.Precio = dto.Precio;
            productoexistente.Disponible = dto.Disponible;

            repository.Update(productoexistente);

            await hubContext.Clients.All.SendAsync("ProductoEditado", mapper.Map<ProductoDTO>(productoexistente));

            return Ok("El producto fue editado con exito.");
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var productoexistente = repository.GetById(id);

            if (productoexistente == null)
            {
                return NotFound("No se pudo encontrar el producto que desea eliminar.");
            }

            repository.Delete(id);

            await hubContext.Clients.All.SendAsync("ProductoEliminado", productoexistente.Id);

            return Ok("El producto fue eliminado con exito.");
        }
    }
}
