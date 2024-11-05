using ApiBRD.Helpers;
using ApiBRD.Hubs;
using ApiBRD.Models.DTOs;
using ApiBRD.Models.Entities;
using ApiBRD.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

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
        private readonly Repository<Categoria> repositoryCategoria;
        public ProductosController(Repository<Categoria> repositoryCategoria, Repository<Producto> repository, IHubContext<CategoriaHub> hubContext, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            this.repository = repository;
            this.hubContext = hubContext;
            this.mapper = mapper;
            this.webHostEnvironment = webHostEnvironment;
            this.repositoryCategoria = repositoryCategoria;
        }

        [HttpGet("Categoria/{id:int}")]
        public async Task<IActionResult> GetProductosByCategoria(int id)
        {
            var categoria = repositoryCategoria.Context.Categoria.Include(x => x.Producto).FirstOrDefault(x => x.Id == id);
            if (categoria == null) return NotFound("Category doesn't exist");
            var dato = categoria.Producto.Select(mapper.Map<ProductoDTO>);
            return Ok(dato);
        }



        [HttpGet]
        public IActionResult GetAll()
        {
            var data = repository.Context.Producto.Include(x => x.IdCategoriaNavigation)
                .Select(x => mapper.Map<ProductoIncludeDTO>(x));
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

            if (!string.IsNullOrWhiteSpace(dto.ImagenBase64))
            {
                try
                {
                    string path = Path.Combine(webHostEnvironment.WebRootPath, "producto", p.Id.ToString());
                    ImagenConverter.ConvertBase64ToImage(dto.ImagenBase64, path);
                }
                catch (Exception ex)
                {
                    repository.Delete(p);
                    return StatusCode(500, "Error al guardar imagen: " + ex.Message);
                }
            }

            await hubContext.Clients.All.SendAsync("NuevoProducto", p);

            return Ok("El producto fue agregado con exito.");
        }

        [HttpGet("disponible")]
        public async Task<IActionResult> UpdateDisponible([FromQuery] int id, [FromQuery] bool disponible)
        {
            var dato = repository.GetById(id);
            if (dato == null)
            {
                return BadRequest("No existe en la base de datos");
            }

            dato.Disponible = disponible;
            repository.Update(dato);
            await hubContext.Clients.All.SendAsync("disponibilidad", new { id = dato.Id, disponibilidad = dato.Disponible });
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> EditarProducto(ProductoImagenDTO dto)
        {
            var productoexistente = repository.GetById(dto.Id);

            if (productoexistente == null)
            {
                return NotFound("El producto que intenta editar no fue encontrado.");
            }
            productoexistente.Nombre = dto.Nombre;
            productoexistente.Precio = dto.Precio;
            productoexistente.Disponible = dto.Disponible;
            productoexistente.IdCategoria = dto.IdCategoria;

            if (dto.ImagenBase64 != null)
            {
                try
                {
                    string path = Path.Combine(webHostEnvironment.WebRootPath, "producto", dto.Id.ToString());
                    ImagenConverter.ConvertBase64ToImage(dto.ImagenBase64, path);
                }
                catch (Exception ex)
                {

                    return StatusCode(500, "Error en la imagen:" + ex.Message);
                }
            }

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
                var men = repository.Context.Menudeldia.FirstOrDefault(x => x.IdProducto == productoexistente.Id) ?? throw new Exception("Error al buscar en menu del dia");
                repository.Context.Set<Menudeldia>().Remove(men);
                repository.Context.SaveChanges();
            }

            repository.Delete(id);

            await hubContext.Clients.All.SendAsync("ProductoEliminado", productoexistente.Id);

            return Ok("El producto fue eliminado con exito.");
        }
    }
}
