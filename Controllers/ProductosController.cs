using ApiBRD.Helpers;
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
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductosController(Repository<Producto> repository, IHubContext<CategoriaHub> hubContext, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            this.repository = repository;
            this.hubContext = hubContext;
            this.mapper = mapper;
            this.webHostEnvironment = webHostEnvironment;
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
        public async Task<IActionResult> AgregarProducto(ProductoImagenDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                return BadRequest("Ingrese un nombre para el producto.");
            }
            if (string.IsNullOrWhiteSpace(dto.ImagenBase64))
            {
                return BadRequest("Envie la imagen en base 64.");

            }

            Producto p = new()
            {
                Nombre = dto.Nombre,
                Precio = dto.Precio,
                Disponible = dto.Disponible,
                IdCategoria = dto.IdCategoria
            };
            repository.Insert(p);

            try
            {
                string path = Path.Combine(webHostEnvironment.WebRootPath, "producto", p.Id.ToString());
                ImagenConverter.ConvertBase64ToImage(dto.ImagenBase64, path);
            }
            catch (Exception ex)
            {
                repository.Delete(p);
                return StatusCode(500, "Error al guardar imagen: " + ex.Message );
            }

            await hubContext.Clients.All.SendAsync("NuevoProducto", p);

            return Ok("El producto fue agregado con exito.");
        }

        [HttpGet("disponble")]
        public async Task<IActionResult> UpdateDisponible([FromQuery] int id, [FromQuery] bool disponible)
        {
            var dato = repository.GetById(id);
            if (dato == null)
            {
                return BadRequest("No existe en la base de datos");
            }

            dato.Disponible = disponible;
            repository.Update(dato);
            return Ok();
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
            Producto productoexistente = repository.GetById(id);

            if (productoexistente == null)
            {
                return NotFound("No se pudo encontrar el producto que desea eliminar.");
            }

            if (repository.Context.Menudeldia.Any(x => x.IdProducto == productoexistente.Id))
            {
                var men = repository.Context.Menudeldia.FirstOrDefault(x => x.IdProducto == productoexistente.Id)??throw new Exception("Error al buscar en menu del dia");
                repository.Context.Set<Menudeldia>().Remove(men);
                repository.Context.SaveChanges();
            }

            repository.Delete(id);

            await hubContext.Clients.All.SendAsync("ProductoEliminado", productoexistente.Id);

            return Ok("El producto fue eliminado con exito.");
        }
    }
}
